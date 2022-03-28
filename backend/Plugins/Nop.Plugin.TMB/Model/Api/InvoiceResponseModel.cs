using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.TMB.Model.Api
{
    public class InvoiceResponseModel
    {
        [JsonProperty("Id")]
        public string GuidId { get; set; }
        public string Status { get; set; }
        [JsonProperty("request_date")]
        public DateTime RequestDate { get; set; }
        [JsonProperty("response_date")]
        public DateTime ResponseDate { get; set; }
        [JsonProperty("last_update")]
        public DateTime LastUpdate { get; set; }
        public List<InvoiceResponseInvoices> Invoices { get; set; }
    }
    public class InvoiceResponseInvoices
    {
        [JsonProperty("transit_code")]
        public string TransitCode { get; set; }
        public string Status { get; set; }
        [JsonProperty("pdf_name")]
        public string PdfName { get; set; }
    }

    /*
     {
            "id": "01128900-06e9-11ea-b9cc-43577ca67b40",
            "status": "PENDING",
            "request_date": "2022-02-07T15:18:30.232Z",
            "response_date": "2022-02-07T15:18:30.232Z",
            "last_update": "2022-02-07T15:18:30.232Z",
            "invoices": [
            {
                "transit_code": "01001021644229877",
                "status": "Success",
                "pdf_name": "IT_005876_22NOIVA-001063.pdf"
            },
            {
                "transit_code": "01002051644219877",
                "status": "Failure"
            },
            {
                "transit_code": "01002031644219769",
                "status": "Duplicate",
                "pdf_name": "FR_69_21.pdf"
            }
            ]
        }
     */
}