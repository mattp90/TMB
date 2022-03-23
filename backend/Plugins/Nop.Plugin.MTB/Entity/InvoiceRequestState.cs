using System;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;

namespace Nop.Plugin.MTB.Entity
{
    public class InvoiceRequestState : BaseEntity, ILocalizedEntity
    {
        public string Description { get; set; }
        
        public DateTime CreatedOnUtc { get; set; }
         
        public DateTime UpdatedOnUtc { get; set; }
    }
}