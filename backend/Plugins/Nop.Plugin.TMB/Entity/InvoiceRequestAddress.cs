using System;
using Nop.Core;
using Nop.Core.Domain.Localization;

namespace Nop.Plugin.TMB.Entity
{
    public class InvoiceRequestAddress : BaseEntity, ILocalizedEntity
    {
        public int InvoiceRequestId { get; set; }
        public string AddressName { get; set; }
        public int? PostalCode { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public virtual InvoiceRequest InvoiceRequest { get; set; }
    }
}
