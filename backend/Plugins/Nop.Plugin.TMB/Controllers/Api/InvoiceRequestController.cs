using System;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Plugin.TMB.Entity;
using Nop.Plugin.TMB.Services.InvoiceRequest;
using Nop.Services.Logging;
using Nopalm;
using Nopalm.Controllers.Api;
using Nop.Plugin.TMB.Extensions;
using Nop.Plugin.TMB.Helpers;
using Nop.Plugin.TMB.Model.Admin.InvoiceRequest;
using Nop.Plugin.TMB.Model.Api;

namespace Nop.Plugin.TMB.Controllers.Api
{
    public class InvoiceRequestController : ApiDefaultController
    {
        private readonly IInvoiceRequestService _invoiceRequestService;
        private readonly ILogger _logger;
        
        public InvoiceRequestController(ApiSettings apiSettings, ILogger logger, IInvoiceRequestService invoiceRequestService) : base(apiSettings, logger)
        {
            _logger = logger;
            _invoiceRequestService = invoiceRequestService;
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
                var jsonContent = JsonConvert.SerializeObject(model);
                var ftp = new FTPManager("ftp://ftpsgeie.aqdemo.it", 5010,"ftp_invoice_demo|ftp_invoice_demo","7ujmNhgg!@jHUf","request","response", "processed");
                ftp.UploadFile($"{entity.GuidId}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.json", jsonContent);
                
                return ApiReturn(
                    new ApiError()
                    {
                        Code = HttpStatusCode.OK,
                        Message = $"ok"
                    });
            }
            catch (Exception e)
            {
                await _logger.ErrorAsync($"Error during executing Nop.Plugin.TMB.Controllers.Api.Create method for model{System.Environment.NewLine}{JsonConvert.SerializeObject(model)}", e);
                
                return ApiReturn(
                    new ApiError()
                    {
                        Code = HttpStatusCode.InternalServerError,
                        Message = $"Error during save request: {e.Message}"
                    });
            }
        }
        
        [HttpGet, Route("/api/testresponse")]
        public virtual async Task<IActionResult> TestResponse([FromBody] InvoiceResponseModel model)
        {
            try
            {
                var request = await _invoiceRequestService.GetDetailByGuidAsync(model.GuidId);
                if (Enum.TryParse(model.Status.ToUpper(), out InvoiceRequestStatusEnum myStatus))
                {
                    request.InvoiceRequestStatusId = (int)myStatus;
                }

                request.RequestDate = model.RequestDate;
                request.ResponseDate = model.ResponseDate;
                request.LastUpdate = model.LastUpdate;
            
                await _invoiceRequestService.UpdateAsync(request);

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
            
                // if status is ... send mail otherwise TO DO
            
                return ApiReturn(
                    new ApiError()
                    {
                        Code = HttpStatusCode.OK,
                        Message = $"ok"
                    });
            }
            catch (Exception e)
            {
                return ApiReturn(
                    new ApiError()
                    {
                        Code = HttpStatusCode.InternalServerError,
                        Message = $"{e.Message}"
                    });
            }
        }
    }
}