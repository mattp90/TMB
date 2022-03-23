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
            entity.InvoiceRequestStateId = (int)InvoiceRequestStateEnum.Pending;
            await _invoiceRequestService.InsertAsync(entity);
            
            var address = model.Address.ToEntity<InvoiceRequestAddress>();
            address.InvoiceRequestId = entity.Id;
            await _invoiceRequestService.InsertAddressAsync(address);

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
            var response = model;
            
            return ApiReturn(
                new ApiError()
                {
                    Code = HttpStatusCode.OK,
                    Message = $"ok"
                });
        }
    }
}