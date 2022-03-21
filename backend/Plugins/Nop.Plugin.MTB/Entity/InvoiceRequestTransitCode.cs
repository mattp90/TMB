using System;
using Nop.Core;
using Nop.Core.Domain.Localization;

namespace Nop.Plugin.MTB.Entity
{
    public class InvoiceRequestTransitCode : BaseEntity, ILocalizedEntity
    {
        public int InvoiceRequestId { get; set; }
        public string Code { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime UpdatedOnUTC { get; set; }
    }
}
