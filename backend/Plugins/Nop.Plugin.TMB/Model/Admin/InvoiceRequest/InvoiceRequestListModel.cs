using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.TMB.Model.Admin.InvoiceRequest
{
    public record InvoiceRequestListModel : BasePagedListModel<InvoiceRequestForGridModel>
    {

    }
    
    public record InvoiceRequestTransitCodeListModel : BasePagedListModel<InvoiceRequestTransitCodeModel>
    {

    }
}
