using System;
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

        public DateTime CreatedOnUTC { get; set; }
         
        public DateTime UpdatedOnUTC { get; set; }
    }
}