using System;
using Newtonsoft.Json;
using Nop.Plugin.TMB.Helpers;
using Nop.Plugin.TMB.Model.Api;
using Nop.Plugin.TMB.Services.InvoiceRequest;
using Nop.Services.Logging;
using Nop.Services.Tasks;

namespace Nop.Plugin.TMB.Task
{
    public partial class CheckInvoiceResponseTask : IScheduleTask
    {
        private readonly ILogger _logger;
        private readonly IInvoiceRequestService _invoiceRequestService;
        
        public CheckInvoiceResponseTask(ILogger logger, IInvoiceRequestService invoiceRequestService)
        {
            _logger = logger;
            _invoiceRequestService = invoiceRequestService;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public virtual async System.Threading.Tasks.Task ExecuteAsync()
        {
            await GetResponses();
            await _logger.InformationAsync("Execution test");
        }

        private async System.Threading.Tasks.Task GetResponses()
        {
            var ftp = new FTPManager("ftp://ftpsgeie.aqdemo.it", 5010,"ftp_invoice_demo|ftp_invoice_demo","7ujmNhgg!@jHUf","request","response", "processed");
            var responses = ftp.GetResponseFiles();

            foreach (var response in responses)
            {
                try
                {
                    var contentJson = ftp.DownloadFile(response);
                    var model = JsonConvert.DeserializeObject<InvoiceResponseModel>(contentJson);
                    var request = await _invoiceRequestService.GetDetailByGuidAsync(model.GuidId);
                    if (Enum.TryParse(model.Status.ToUpper(), out InvoiceRequestStatusEnum myStatus))
                    {
                        request.InvoiceRequestStatusId = (int)myStatus;
                    }

                    request.RequestDate = model.RequestDate;
                    request.ResponseDate = model.ResponseDate;
                    request.LastUpdate = model.LastUpdate;
            
                    // Update info InvoiceRequest
                    await _invoiceRequestService.UpdateAsync(request);

                    // Update info TransitCodes
                    foreach (var code in model.Invoices)
                    {
                        var transitCode = await _invoiceRequestService.GetTransitCodesByRequestIdAndCode(request.Id, code.TransitCode);
                        transitCode.PdfName = code.PdfName;
                        if (Enum.TryParse(code.Status.ToUpper(), out InvoiceRequestTransitCodeStatusEnum statusCode))
                        {
                            transitCode.InvoiceRequestTransitCodeStatusId = (int)statusCode;
                        }
                
                        await _invoiceRequestService.UpdateTransitCodeAsync(transitCode);
                    }
            
                    // if status is COMPLETED send mail otherwise TODO???
                    if (request.InvoiceRequestStatusId == (int)InvoiceRequestStatusEnum.COMPLETED)
                    {
                        // Send email to customer
                    }
                
                    ftp.MoveFileToProcessed(response);
                }
                catch (Exception e)
                {
                    await _logger.ErrorAsync($"Error during executing Nop.Plugin.TMB.Task.GetResponses method for response {response}", e);
                }
            }
        }
    }
}