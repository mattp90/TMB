using System;
using Nop.Core;
using Nop.Core.Domain.Localization;

namespace Nop.Plugin.MTB.Entity
{
    public class InvoiceRequestTransitCode : BaseEntity, ILocalizedEntity
    {
        public int InvoiceRequestId { get; set; }
        public string Code { get; set; }

        public int? InvoiceRequestTransitionCodeStateId { get; set; }

        public string PdfName { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        
        public virtual InvoiceRequestTransitCodeState InvoiceRequestTransitionCodeState { get; set; }
        public virtual InvoiceRequest InvoiceRequest { get; set; }
    }
}
