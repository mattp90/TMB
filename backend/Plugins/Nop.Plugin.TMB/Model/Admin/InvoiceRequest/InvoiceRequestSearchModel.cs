using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.TMB.Model.Admin.InvoiceRequest
{
    public record InvoiceRequestSearchModel : BaseSearchModel
    {
        public InvoiceRequestSearchModel()
        {
            SearchStates = new List<SelectListItem>();
        }
        
        public int Id { get; set; }

        [NopResourceDisplayName(TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.Name")]
        public string SearchName { get; set; }
        
        [NopResourceDisplayName(TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.Surname")]
        public string SearchSurname { get; set; }
        
        [NopResourceDisplayName(TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.BusinessName")]
        public string SearchBusinessName { get; set; }
        
        [NopResourceDisplayName(TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.FiscalCode")]
        public string SearchFiscalCode { get; set; }
        
        [NopResourceDisplayName(TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.PEC")]
        public string SearchPEC { get; set; }

        [NopResourceDisplayName(TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.TransitCode")]
        public string SearchTransitCode { get; set; }
        
        [NopResourceDisplayName(TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.Status")]
        public int SearchStateId { get; set; }
        public IList<SelectListItem> SearchStates { get; set; }
    }
}
