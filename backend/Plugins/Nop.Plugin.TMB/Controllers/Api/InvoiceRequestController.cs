using System;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core.Configuration;
using Nop.Plugin.TMB.Entity;
using Nop.Plugin.TMB.Services.InvoiceRequest;
using Nop.Services.Logging;
using Nopalm;
using Nopalm.Controllers.Api;
using Nop.Plugin.TMB.Extensions;
using Nop.Plugin.TMB.Helpers;
using Nop.Plugin.TMB.Model.Admin.InvoiceRequest;

namespace Nop.Plugin.TMB.Controllers.Api
{
    public class InvoiceRequestController : ApiDefaultController
    {
        private readonly AppSettings _appSettings;
        private readonly IInvoiceRequestService _invoiceRequestService;
        private readonly ILogger _logger;
        
        public InvoiceRequestController(AppSettings appSettings, ApiSettings apiSettings, ILogger logger, IInvoiceRequestService invoiceRequestService) : base(apiSettings, logger)
        {
            _logger = logger;
            _invoiceRequestService = invoiceRequestService;
            _appSettings = appSettings;
        }

        [HttpGet, Route("/api/ping")]
        public virtual IActionResult Ping()
        {
            return ApiReturn(
                new ApiError
                {
                    Code = HttpStatusCode.OK,
                    Message = $"ok"
                });
        }
        
        [HttpPost, Route("/api/createinvoicerequest")]
        public virtual async Task<IActionResult> Create([FromBody] InvoiceRequestModel model)
        {
            try
            {
                var entity = model.ToEntity<InvoiceRequest>();
                entity.GuidId = Guid.NewGuid();
                entity.RequestDate = DateTime.Now;
                entity.LastUpdate = DateTime.Now;
                await _invoiceRequestService.InsertAsync(entity);
            
                var address = model.InvoiceRequestAddress.ToEntity<InvoiceRequestAddress>();
                address.InvoiceRequestId = entity.Id;
                await _invoiceRequestService.InsertAddressAsync(address);

                var fiscalId = model.InvoiceRequestFiscalId.ToEntity<InvoiceRequestFiscalId>();
                fiscalId.InvoiceRequestId = entity.Id;
                await _invoiceRequestService.InsertFiscalIdAsync(fiscalId);
            
                foreach (string transitCode in model.TransitCodes)
                {
                    await _invoiceRequestService.InsertTransitCode(new InvoiceRequestTransitCode()
                    {
                        Code = transitCode,
                        InvoiceRequestId = entity.Id,
                        CreatedOnUtc = DateTime.Now,
                        UpdatedOnUtc = DateTime.Now
                    });
                }

                // Send to FTP folder
                model.GuidId = entity.GuidId;
                var jsonContent = JsonConvert.SerializeObject(model, Formatting.Indented);
                var ftp = new FTPManager(_appSettings.FtpConfig.Host, _appSettings.FtpConfig.Port,_appSettings.FtpConfig.Username,_appSettings.FtpConfig.Password,_appSettings.FtpConfig.RequestFolder,
                    _appSettings.FtpConfig.ResponseFolder, _appSettings.FtpConfig.ProcessedFolder, _appSettings.FtpConfig.PdfFolder);
                ftp.UploadFile($"{entity.GuidId}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.json", jsonContent);

                await _logger.InformationAsync($"Inser new invoice request with id: {entity.Id}");
                
                return ApiReturn(
                    new ApiError()
                    {
                        Code = HttpStatusCode.OK,
                        Message = $"ok"
                    });
            }
            catch (Exception e)
            {
                await _logger.ErrorAsync($"Error during executing Nop.Plugin.TMB.Controllers.Api.Create method for model{System.Environment.NewLine}{JsonConvert.SerializeObject(model)}", exception: e);
                
                return ApiReturn(
                    new ApiError()
                    {
                        Code = HttpStatusCode.InternalServerError,
                        Message = $"Error during save request: {e.Message}"
                    });
            }
        }
    }
}