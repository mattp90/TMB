using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.MTB.Model.Admin.InvoiceRequest
{
    public record InvoiceRequestSearchModel : BaseSearchModel
    {
        public int Id { get; set; }

        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.Name")]
        public string SearchName { get; set; }
        
        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.Surname")]
        public string SearchSurname { get; set; }
        
        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.BusinessName")]
        public string SearchBusinessName { get; set; }
        
        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.FiscalCode")]
        public string SearchFiscalCode { get; set; }
        
        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.PEC")]
        public string SearchPEC { get; set; }

        [NopResourceDisplayName(MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.TransitCode")]
        public string SearchTransitCode { get; set; }
    }
}
