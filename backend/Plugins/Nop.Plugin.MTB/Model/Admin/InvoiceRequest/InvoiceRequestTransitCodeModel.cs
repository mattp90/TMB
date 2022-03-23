using System;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.MTB.Model.Admin.InvoiceRequest
{
    public record InvoiceRequestTransitCodeModel : BaseNopEntityModel
    {
        public int InvoiceRequestId { get; set; }
        public string Code { get; set; }
        public string InvoiceRequestTransitionCodeStateDescription { get; set; }
        public string PdfName { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}