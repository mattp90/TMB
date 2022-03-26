using System;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.MTB.Entity;
using Nop.Plugin.MTB.Services.InvoiceRequest;
using Nop.Services.Logging;
using Nopalm;
using Nopalm.Controllers.Api;
using Nop.Plugin.MTB.Extensions;
using Nop.Plugin.MTB.Helpers;
using Nop.Plugin.MTB.Model.Admin.InvoiceRequest;
using Nop.Plugin.MTB.Model.Api;

namespace Nop.Plugin.MTB.Controllers.Api
{
    public class InvoiceRequestController : ApiDefaultController
    {
        private readonly IInvoiceRequestService _invoiceRequestService;
        
        public InvoiceRequestController(ApiSettings apiSettings, ILogger logger, IInvoiceRequestService invoiceRequestService) : base(apiSettings, logger)
        {
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
        
        [HttpGet, Route("/api/createinvoicerequest")]
        public virtual async Task<IActionResult> Create([FromBody] InvoiceRequestModel model)
        {
            var entity = model.ToEntity<InvoiceRequest>();
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
            
            return ApiReturn(
                new ApiError()
                {
                    Code = HttpStatusCode.OK,
                    Message = $"ok"
                });
        }
        
        [HttpGet, Route("/api/testresponse")]
        public virtual async Task<IActionResult> TestResponse([FromBody] InvoiceResponseModel model)
        {
            var request = await _invoiceRequestService.GetDetailByGuidAsync(model.GuidId);
            if (Enum.TryParse(model.Status.ToUpper(), out InvoiceRequestStateEnum myStatus))
            {
                request.InvoiceRequestStateId = (int)myStatus;
            }

            request.RequestDate = model.RequestDate;
            request.ResponseDate = model.ResponseDate;
            request.LastUpdate = model.LastUpdate;
            
            await _invoiceRequestService.UpdateAsync(request);

            foreach (var code in model.Invoices)
            {
                var transitCode = await _invoiceRequestService.GetTransitCodesByRequestIdAndCode(request.Id, code.TransitCode);
                transitCode.PdfName = code.PdfName;
                if (Enum.TryParse(code.Status.ToUpper(), out InvoiceRequestTransitCodeStateEnum statusCode))
                {
                    transitCode.InvoiceRequestTransitCodeStateId = (int)statusCode;
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
    }
}