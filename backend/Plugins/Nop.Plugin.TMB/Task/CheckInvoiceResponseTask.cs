using System;
using Newtonsoft.Json;
using Nop.Core.Configuration;
using Nop.Plugin.TMB.Helpers;
using Nop.Plugin.TMB.Model.Api;
using Nop.Plugin.TMB.Services.InvoiceRequest;
using Nop.Services.Tasks;
using Microsoft.Extensions.Logging;
using Nop.Core.Domain.Messages;
using Nop.Services.Localization;
using Nop.Services.Messages;
using System.Linq;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Plugin.TMB.Entity;
using Nop.Services.Media;

namespace Nop.Plugin.TMB.Task
{
    public partial class CheckInvoiceResponseTask : IScheduleTask
    {
        private readonly AppSettings _appSettings;
        private readonly IInvoiceRequestService _invoiceRequestService;
        private readonly ILogger<CheckInvoiceResponseTask> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ITokenizer _tokenizer;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IDownloadService _downloadService;
        private readonly INopFileProvider _fileProvider;
        
        public CheckInvoiceResponseTask(AppSettings appSettings, ILogger<CheckInvoiceResponseTask> logger, IInvoiceRequestService invoiceRequestService, ILocalizationService localizationService, IQueuedEmailService queuedEmailService, ITokenizer tokenizer, IMessageTemplateService messageTemplateService, IEmailAccountService emailAccountService, EmailAccountSettings emailAccountSettings, IDownloadService downloadService, INopFileProvider fileProvider)
        {
            _logger = logger;
            _invoiceRequestService = invoiceRequestService;
            _localizationService = localizationService;
            _queuedEmailService = queuedEmailService;
            _tokenizer = tokenizer;
            _messageTemplateService = messageTemplateService;
            _emailAccountService = emailAccountService;
            _emailAccountSettings = emailAccountSettings;
            _downloadService = downloadService;
            _fileProvider = fileProvider;
            _appSettings = appSettings;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public virtual async System.Threading.Tasks.Task ExecuteAsync()
        {
            await GetResponses();
        }

        private async System.Threading.Tasks.Task GetResponses()
        {
            var ftp = new FTPManager(_appSettings.FtpConfig.Host, _appSettings.FtpConfig.Port,_appSettings.FtpConfig.Username,_appSettings.FtpConfig.Password,_appSettings.FtpConfig.RequestFolder,
                _appSettings.FtpConfig.ResponseFolder, _appSettings.FtpConfig.ProcessedFolder, _appSettings.FtpConfig.PdfFolder);
            var responses = ftp.GetResponseFiles();

            _logger.LogInformation($"Loaded {responses.Count} responses to processing");
            
            foreach (var response in responses)
            {
                try
                {
                    _logger.LogInformation($"\tProcessing {response} file");
                    
                    var contentJson = ftp.DownloadFile(response);
                    var model = JsonConvert.DeserializeObject<InvoiceResponseModel>(contentJson);
                    var request = await _invoiceRequestService.GetDetailByGuidAsync(model.GuidId);
                    if (Enum.TryParse(model.Status.ToUpper(), out InvoiceRequestStatusEnum myStatus))
                    {
                        request.InvoiceRequestStatusId = (int)myStatus;
                    }

                    request.ResponseDate = model.ResponseDate;
                    request.LastUpdate = DateTime.Now;
            
                    // Update info InvoiceRequest
                    await _invoiceRequestService.UpdateAsync(request);
                    request.InvoiceRequestTransitCode = new List<InvoiceRequestTransitCode>();
                    
                    // Update info TransitCodes
                    foreach (var code in model.Invoices)
                    {
                        var transitCode = await _invoiceRequestService.GetTransitCodesByRequestIdAndCode(request.Id, code.TransitCode);
                        transitCode.PdfName = code.PdfName;
                        if (Enum.TryParse(code.Status.ToUpper(), out InvoiceRequestTransitCodeStatusEnum statusCode))
                        {
                            transitCode.InvoiceRequestTransitCodeStatusId = (int)statusCode;
                            await _invoiceRequestService.UpdateTransitCodeAsync(transitCode);
                            
                            request.InvoiceRequestTransitCode.Add(transitCode);
                        }
                    }
            
                    // if status is COMPLETED send mail
                    if (request.InvoiceRequestStatusId == (int)InvoiceRequestStatusEnum.COMPLETED)
                    {
                        _logger.LogInformation($"\t\tSending email with pdf attach");
                        
                        foreach (var transitCode in request.InvoiceRequestTransitCode)
                        {
                            if (transitCode.InvoiceRequestTransitCodeStatusId is (int)InvoiceRequestTransitCodeStatusEnum.SUCCESS or (int)InvoiceRequestTransitCodeStatusEnum.DUPLICATE)
                            {
                                // Get file pdf from ftp server, upload in nop and send mail with attachment
                                var attach = await UploadPdfInvoiceInNop(transitCode.PdfName);
                                // Send email to customer
                                await SendTestEmailAsync( request.Email, transitCode.Code, attach);
                            }
                        }
                    }
                
                    ftp.MoveFileToProcessed(response);
                    
                    _logger.LogInformation($"\t{response} processed correctly");
                }
                catch (Exception ex)
                {
                    _logger.LogError(message:$"Error during executing Nop.Plugin.TMB.Task.GetResponses method for response {response}", exception: ex);
                }
            }
        }
        
        public virtual async System.Threading.Tasks.Task<int> SendTestEmailAsync(string toAddress, string transitCode, Download attach, int languageId = 0)
        {
            var messageTemplate = (await _messageTemplateService.GetMessageTemplatesByNameAsync(MessageTemplateSystemNames.InvoiceResponseMessage)).FirstOrDefault();
            if (messageTemplate == null)
                throw new ArgumentException("Template cannot be loaded");

            //email account
            var emailAccount = await GetEmailAccountOfMessageTemplateAsync(messageTemplate, languageId);

            //event notification
            // await _eventPublisher.MessageTokensAddedAsync(messageTemplate, tokens);

            var tokens = new List<Token>()
            {
                new Token("InvoiceRequestTransitCode.Code", transitCode)
            };
            
            //force sending
            messageTemplate.DelayBeforeSend = null;

            return await SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens, toAddress, null, attachmentFileName: attach.Filename, attachDownloadId: attach.Id);
        }
        
        public virtual async System.Threading.Tasks.Task<int> SendNotificationAsync(MessageTemplate messageTemplate,
            EmailAccount emailAccount, int languageId, IList<Token> tokens,
            string toEmailAddress, string toName,
            string attachmentFilePath = null, string attachmentFileName = null,
            string replyToEmailAddress = null, string replyToName = null,
            string fromEmail = null, string fromName = null, string subject = null,
            int? attachDownloadId = null)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException(nameof(messageTemplate));

            if (emailAccount == null)
                throw new ArgumentNullException(nameof(emailAccount));

            //retrieve localized message template data
            var bcc = await _localizationService.GetLocalizedAsync(messageTemplate, mt => mt.BccEmailAddresses, languageId);
            if (string.IsNullOrEmpty(subject))
                subject = await _localizationService.GetLocalizedAsync(messageTemplate, mt => mt.Subject, languageId);
            var body = await _localizationService.GetLocalizedAsync(messageTemplate, mt => mt.Body, languageId);

            //Replace subject and body tokens 
            var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
            var bodyReplaced = _tokenizer.Replace(body, tokens, true);

            //limit name length
            toName = CommonHelper.EnsureMaximumLength(toName, 300);

            var email = new QueuedEmail
            {
                Priority = QueuedEmailPriority.High,
                From = !string.IsNullOrEmpty(fromEmail) ? fromEmail : emailAccount.Email,
                FromName = !string.IsNullOrEmpty(fromName) ? fromName : emailAccount.DisplayName,
                To = toEmailAddress,
                ToName = toName,
                ReplyTo = replyToEmailAddress,
                ReplyToName = replyToName,
                CC = string.Empty,
                Bcc = bcc,
                Subject = subjectReplaced,
                Body = bodyReplaced,
                AttachmentFilePath = attachmentFilePath,
                AttachmentFileName = attachmentFileName,
                AttachedDownloadId = (attachDownloadId.HasValue) ? attachDownloadId.Value : messageTemplate.AttachedDownloadId,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccount.Id,
                DontSendBeforeDateUtc = !messageTemplate.DelayBeforeSend.HasValue ? null
                    : (DateTime?)(DateTime.UtcNow + TimeSpan.FromHours(messageTemplate.DelayPeriod.ToHours(messageTemplate.DelayBeforeSend.Value)))
            };

            await _queuedEmailService.InsertQueuedEmailAsync(email);
            return email.Id;
        }
        
        protected virtual async System.Threading.Tasks.Task<EmailAccount> GetEmailAccountOfMessageTemplateAsync(MessageTemplate messageTemplate, int languageId)
        {
            var emailAccountId = await _localizationService.GetLocalizedAsync(messageTemplate, mt => mt.EmailAccountId, languageId);
            //some 0 validation (for localizable "Email account" dropdownlist which saves 0 if "Standard" value is chosen)
            if (emailAccountId == 0)
                emailAccountId = messageTemplate.EmailAccountId;

            var emailAccount = (await _emailAccountService.GetEmailAccountByIdAsync(emailAccountId) ?? await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId)) ??
                               (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();
            return emailAccount;
        }
        
        public virtual async System.Threading.Tasks.Task<Download> UploadPdfInvoiceInNop(string pdfName)
        {   
            var ftp = new FTPManager(_appSettings.FtpConfig.Host, _appSettings.FtpConfig.Port,_appSettings.FtpConfig.Username,_appSettings.FtpConfig.Password,_appSettings.FtpConfig.RequestFolder,
                _appSettings.FtpConfig.ResponseFolder, _appSettings.FtpConfig.ProcessedFolder, _appSettings.FtpConfig.PdfFolder);
            
            var fileBinary = ftp.DownloadInvoicePdf(pdfName);
            
            var fileExtension = _fileProvider.GetFileExtension(pdfName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();
            
            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = string.Empty,
                DownloadBinary = fileBinary,
                ContentType = "application/pdf",
                //we store filename without extension for downloads
                Filename = _fileProvider.GetFileNameWithoutExtension(pdfName),
                Extension = fileExtension,
                IsNew = true
            };
            
            await _downloadService.InsertDownloadAsync(download);
            return download;
        }   
    }
}