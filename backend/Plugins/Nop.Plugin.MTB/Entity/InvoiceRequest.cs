using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;

namespace Nop.Plugin.MTB.Entity
{
    public class InvoiceRequest : BaseEntity, ILocalizedEntity, ISoftDeletedEntity
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string BusinessName { get; set; }

        public string FiscalCode { get; set; }

        public string PEC { get; set; }
        public bool Deleted { get; set; }

        public int InvoiceRequestStateId { get; set; }

        public DateTime? RequestDate { get; set; }

        public DateTime? LastUpdate { get; set; }

        public DateTime? ResponseDate { get; set; }

        public DateTime CreatedOnUtc { get; set; }
                                  
        public DateTime UpdatedOnUtc { get; set; }

        public virtual InvoiceRequestState InvoiceRequestState { get; set; }

        public virtual InvoiceRequestAddress InvoiceRequestAddress { get; set; }

        public virtual List<InvoiceRequestTransitCode> InvoiceRequestTransitCode { get; set; }
    }
}