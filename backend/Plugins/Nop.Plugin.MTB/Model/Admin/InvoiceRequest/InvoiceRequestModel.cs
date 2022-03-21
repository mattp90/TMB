using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Nop.Plugin.MTB.Model.Admin.InvoiceRequest
{
    public record InvoiceRequestModel : BaseNopEntityModel
    {
        public string Name { get; set; }
        
        public string Surname { get; set; }
        
        [JsonProperty("business_name")]
        public string BusinessName { get; set; }

        public InvoiceRequestAddressModel Address { get; set; }
        
        [JsonProperty("fiscal_code")]
        public string FiscalCode { get; set; }
        
        public string PEC { get; set; }
        
        [JsonProperty("transit_codes")]
        public List<string> TransitCodes { get; set; }
        
        public DateTime CreatedOnUTC { get; set; }
        public DateTime UpdatedOnUTC { get; set; }
    }
    
    public record InvoiceRequestAddressModel : BaseNopEntityModel
    {
        [JsonProperty("address")]
        public string AddressName { get; set; }
        public int PostalCode { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime UpdatedOnUTC { get; set; }
    }
    
    public record InvoiceRequestForGridModel : BaseNopEntityModel
    {
        public string Name { get; set; }
        
        public string Surname { get; set; }
        
        public string BusinessName { get; set; }

        public InvoiceRequestAddressModel Address { get; set; }
        
        public string FiscalCode { get; set; }
        
        public string PEC { get; set; }
        
        public List<string> TransitCodes { get; set; }
        
        public DateTime CreatedOnUTC { get; set; }
        public DateTime UpdatedOnUTC { get; set; }
    }
}


/*
 {
    "id": "01128900-06e9-11ea-b9cc-43577ca67b40",
    "name": "Charles",
    "surname": "Du Pont",
    "business_name": "CASADEI S.p.A.",
    "address": {
        "address": "Via Monte Vodice, 27"
        "postal_code": 11100,
        "city": "Aosta",
        "province": "AO",
        "country": "IT"
    },
    "fiscal_code": "01249040070",    
    "PEC": "charles.dupont@pec.it",    
    "request_date": "2022-02-07T15:18:30.232Z",
    "last_update": "2022-02-07T15:18:30.232Z",
    "transit_codes": [
        "01001021644229877",
        "01002051644219877",
        "01002031644219769",
    ]
}
*/