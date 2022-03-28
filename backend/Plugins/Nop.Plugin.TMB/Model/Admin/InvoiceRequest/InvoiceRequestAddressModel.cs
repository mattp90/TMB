using System;
using Newtonsoft.Json;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.TMB.Model.Admin.InvoiceRequest
{
    public record InvoiceRequestAddressModel : BaseNopEntityModel
    {
        [Newtonsoft.Json.JsonIgnore]
        public int InvoiceRequestId { get; set; }
        
        [NopResourceDisplayName(TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.AddressName")]
        [JsonProperty("address")]
        public string AddressName { get; set; }
        
        [NopResourceDisplayName(TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.PostalCode")]
        [JsonProperty("postal_code")]
        public int? PostalCode { get; set; }
        
        [NopResourceDisplayName(TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.City")]
        [JsonProperty("city")]
        public string City { get; set; }
        
        [NopResourceDisplayName(TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.Province")]
        [JsonProperty("province")]
        public string Province { get; set; }
        
        [NopResourceDisplayName(TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.Country")]
        [JsonProperty("country")]
        public string Country { get; set; }
        
        [Newtonsoft.Json.JsonIgnore]
        public DateTime CreatedOnUtc { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public DateTime UpdatedOnUtc { get; set; }
        
        public override int Id { get; set; }
        
        public bool ShouldSerializeId()
        {
            return false;
        }
    }
}