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
        public InvoiceRequestModel()
        {
            InvoiceRequestSearchModel = new InvoiceRequestSearchModel();
        }
        
        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.Name")]
        public string Name { get; set; }
        
        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.Surname")]
        public string Surname { get; set; }
        
        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.BusinessName")]
        [JsonProperty("business_name")]
        public string BusinessName { get; set; }

        
        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.FiscalCode")]
        [JsonProperty("fiscal_code")]
        public string FiscalCode { get; set; }
        
        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.PEC")]
        public string PEC { get; set; }
        
        [JsonProperty("transit_codes")]
        public List<string> TransitCodes { get; set; }

        [JsonProperty("request_date")]
        public DateTime RequestDate { get; set; }
        
        [JsonProperty("last_update")]
        public DateTime LastUpdate { get; set; }

        public int? InvoiceRequestStateId { get; set; }
        
        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.Status")]
        public string InvoiceRequestStateDescription { get; set; }

        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        
        public InvoiceRequestAddressModel Address { get; set; }
        public List<InvoiceRequestTransitCodeModel> TransitCodesList { get; set; }

        public InvoiceRequestSearchModel InvoiceRequestSearchModel { get; set; }
    }
    
    /// <summary>
    /// Model only for grid. This for problem with json property name with underscores.
    /// </summary>
    public record InvoiceRequestForGridModel : BaseNopEntityModel
    {
        public string Name { get; set; }
        
        public string Surname { get; set; }
        
        public string BusinessName { get; set; }

        public string FiscalCode { get; set; }
        
        public string PEC { get; set; }

        public int? InvoiceRequestStateId { get; set; }
        public string InvoiceRequestStateDescription { get; set; }
    }
}