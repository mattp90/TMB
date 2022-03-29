using System;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.TMB.Model.Admin.InvoiceRequest
{
    public record InvoiceRequestTransitCodeModel : BaseNopEntityModel
    {
        public int InvoiceRequestId { get; set; }
        public string Code { get; set; }
        public int? InvoiceRequestTransitCodeStatusId { get; set; }
        public string InvoiceRequestTransitCodeStatusDescription { get; set; }
        public string PdfName { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}