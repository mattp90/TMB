using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.MTB.Helpers;
using Nop.Plugin.MTB.Model.Admin.InvoiceRequest;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using System.Threading.Tasks;
using Nop.Plugin.MTB.Extensions;
using Nop.Plugin.MTB.Services.InvoiceRequest;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.MTB.Controllers.Admin
{
    public class InvoiceRequestController : BaseAdminController
    {
        private readonly IPermissionService _permissionService;
        private readonly IInvoiceRequestService _invoiceRequestService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        public InvoiceRequestController(IPermissionService permissionService, IInvoiceRequestService invoiceRequestService, ICustomerActivityService customerActivityService, ILocalizationService localizationService, INotificationService notificationService)
        {
            _permissionService = permissionService;            
            _invoiceRequestService = invoiceRequestService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public async Task<ActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(PermissionHelper.AccessFileVintageUpload))
                return new UnauthorizedResult();

            var model = new InvoiceRequestSearchModel();
            // model.SetGridPageSize();
            return View(model);
        }

        [HttpPost]
        public virtual async Task<ActionResult> List(InvoiceRequestSearchModel model)
        {
            if (!await _permissionService.AuthorizeAsync(PermissionHelper.AccessFileVintageUpload))
                return new UnauthorizedResult();

            var invoiceRequestItems = await _invoiceRequestService.GetAllAsync(model.Id, model.SearchName, model.SearchSurname, model.SearchBusinessName, model.SearchFiscalCode, 
                model.SearchPEC, model.SearchTransitCode, model.Page, model.PageSize, noCache: true);
            
            var gridModel = new InvoiceRequestListModel().PrepareToGrid(model, invoiceRequestItems, () =>
            {
                return invoiceRequestItems.Select(f => f.ToModel<InvoiceRequestForGridModel>());
            });            

            return Json(gridModel);
        }
        
        [HttpPost]
        public virtual async Task<IActionResult> Update(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a blog post with the specified id
            var invoiceRequest = await _invoiceRequestService.GetByIdAsync(id);
            if (invoiceRequest == null)
                return RedirectToAction("List");

            await _invoiceRequestService.UpdateAsync(invoiceRequest);

            //activity log
            await _customerActivityService.InsertActivityAsync("UpdateInvoiceRequest",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.UpdateInvoiceRequest"), invoiceRequest.Id), invoiceRequest);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.InvoiceRequest.Updated"));

            return RedirectToAction("List");
        }
        
        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a blog post with the specified id
            var invoiceRequest = await _invoiceRequestService.GetByIdAsync(id);
            if (invoiceRequest == null)
                return RedirectToAction("List");

            await _invoiceRequestService.DeleteAsync(invoiceRequest);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteInvoiceRequest",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.UpdateInvoiceRequest"), invoiceRequest.Id), invoiceRequest);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.InvoiceRequest.Deleted"));

            return RedirectToAction("List");
            
        }
    }
} 
