using System;
using Newtonsoft.Json;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.MTB.Model.Admin.InvoiceRequest
{
    public record InvoiceRequestAddressModel : BaseNopEntityModel
    {
        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.AddressName")]
        [JsonProperty("address")]
        public string AddressName { get; set; }
        
        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.PostalCode")]
        [JsonProperty("postal_code")]
        public int? PostalCode { get; set; }
        
        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.City")]
        public string City { get; set; }
        
        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.Province")]
        public string Province { get; set; }
        
        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.Country")]
        public string Country { get; set; }
        
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}