using System;
using Newtonsoft.Json;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.MTB.Model.Admin.InvoiceRequest
{
    public record InvoiceRequestFiscalIdModel : BaseNopEntityModel
    {
        [Newtonsoft.Json.JsonIgnore]
        public int InvoiceRequestId { get; set; }
        
        [JsonProperty("country")]
        public string Country { get; set; }
        
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("VAT_code")]
        public string VatCode { get; set; }
        
        public override int Id { get; set; }
        
        public bool ShouldSerializeId()
        {
            return false;
        }
    }
}