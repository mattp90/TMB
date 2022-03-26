using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Nop.Plugin.MTB.Entity;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.MTB.Model.Admin.InvoiceRequest
{
    public record InvoiceRequestModel : BaseNopEntityModel
    {
        [JsonProperty("id")]
        public Guid GuidId { get; set; }
        
        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.Name")]
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.Surname")]
        [JsonProperty("surname")]
        public string Surname { get; set; }
        
        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.BusinessName")]
        [JsonProperty("business_name")]
        public string BusinessName { get; set; }
        
        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.PEC")]
        public string PEC { get; set; }
        
        [JsonProperty("transit_codes")]
        public List<string> TransitCodes { get; set; }

        [JsonProperty("SdI_code")]
        public string SdICode { get; set; }

        [JsonProperty("request_date")]
        public DateTime RequestDate { get; set; }
        
        [JsonProperty("last_update")]
        public DateTime LastUpdate { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public int? InvoiceRequestStateId { get; set; }
        
        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.Status")]
        [Newtonsoft.Json.JsonIgnore]
        public string InvoiceRequestStateDescription { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public DateTime CreatedOnUtc { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public DateTime UpdatedOnUtc { get; set; }
        
        [JsonProperty("address")]
        public InvoiceRequestAddressModel InvoiceRequestAddress { get; set; }
        
        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.FiscalCode")]
        [JsonProperty("fiscal_id")]
        public InvoiceRequestFiscalIdModel InvoiceRequestFiscalId { get; set; }

        // [Newtonsoft.Json.JsonIgnore]        
        // public List<InvoiceRequestTransitCodeModel> TransitCodesList { get; set; }
        
        [Newtonsoft.Json.JsonIgnore]
        public InvoiceRequestSearchModel InvoiceRequestSearchModel { get; set; }

        public override int Id { get; set; }
        
        public bool ShouldSerializeId()
        {
            return false;
        }
    }
    
    /// <summary>
    /// Model only for grid. This for problem with json property name with underscores.
    /// </summary>
    public record InvoiceRequestForGridModel : BaseNopEntityModel
    {
        public Guid GuidId { get; set; }
        public string Name { get; set; }
        
        public string Surname { get; set; }
        
        public string BusinessName { get; set; }

        public string FiscalCode { get; set; }
        
        public string PEC { get; set; }

        public int? InvoiceRequestStateId { get; set; }
        public string InvoiceRequestStateDescription { get; set; }
    }
}