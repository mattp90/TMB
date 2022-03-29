using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.TMB.Model.Api
{
    public class InvoiceResponseModel
    {
        [JsonProperty("Id")]
        public Guid GuidId { get; set; }
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
}