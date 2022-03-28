using System;
using Nop.Core;
using Nop.Core.Domain.Localization;

namespace Nop.Plugin.TMB.Entity
{
    public class InvoiceRequestFiscalId : BaseEntity, ILocalizedEntity
    {
        public int InvoiceRequestId { get; set; }
        public string Country { get; set; }
        public string Code { get; set; }
        public string VatCode { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}