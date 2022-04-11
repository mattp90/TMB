using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Nop.Plugin.TMB.Entity;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.TMB.Model.Admin.InvoiceRequest
{
    public record InvoiceRequestModel : BaseNopEntityModel
    {
        [JsonProperty("id")]
        public Guid GuidId { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("surname")]
        public string Surname { get; set; }
        
        [JsonProperty("business_name")]
        public string BusinessName { get; set; }
        
        public string Email { get; set; }
        
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
        public int? InvoiceRequestStatusId { get; set; }
        
        [NopResourceDisplayName(TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.Status")]
        [Newtonsoft.Json.JsonIgnore]
        public string InvoiceRequestStatusDescription { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public DateTime CreatedOnUtc { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public DateTime UpdatedOnUtc { get; set; }

        public string Culture { get; set; }

        [JsonProperty("address")]
        public InvoiceRequestAddressModel InvoiceRequestAddress { get; set; }
        
        [JsonProperty("fiscal_id")]
        public InvoiceRequestFiscalIdModel InvoiceRequestFiscalId { get; set; }

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
        
        [NopResourceDisplayName(TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.Name")]
        public string Name { get; set; }
        
        [NopResourceDisplayName(TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.Surname")]
        public string Surname { get; set; }
        
        [NopResourceDisplayName(TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.BusinessName")]
        public string BusinessName { get; set; }

        [NopResourceDisplayName(TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.RequestDate")]
        public string RequestDate { get; set; }
        
        [NopResourceDisplayName(TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.ResponseDate")]
        public string ResponseDate { get; set; }

        [NopResourceDisplayName(TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.Email")]
        public string Email { get; set; }
        
        [NopResourceDisplayName(TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.PEC")]
        public string PEC { get; set; }

        public int? InvoiceRequestStatusId { get; set; }
        
        [NopResourceDisplayName(TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.Status")]
        public string InvoiceRequestStatusDescription { get; set; }
    }
}