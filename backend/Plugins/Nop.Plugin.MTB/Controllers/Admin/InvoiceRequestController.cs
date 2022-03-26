using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.MTB.Helpers;
using Nop.Plugin.MTB.Model.Admin.InvoiceRequest;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Plugin.MTB.Extensions;
using Nop.Plugin.MTB.Services.InvoiceRequest;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Filters;

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
            model.SetGridPageSize();
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
        public virtual async Task<ActionResult> ListTransitCode(InvoiceRequestSearchModel model)
        {
            if (!await _permissionService.AuthorizeAsync(PermissionHelper.AccessFileVintageUpload))
                return new UnauthorizedResult();

            var codes = await _invoiceRequestService.GetTransitCodesByIdRequestAsync(model.Id);
            
            var gridModel = new InvoiceRequestTransitCodeListModel().PrepareToGrid(model, codes, () =>
            {
                return codes.Select(f => f.ToModel<InvoiceRequestTransitCodeModel>());
            });            

            return Json(gridModel);
        }
        
        public async Task<ActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(PermissionHelper.AccessFileVintageUpload))
                return Content("Access denied");

            var item = await _invoiceRequestService.GetDetailByIdAsync(id);
            if (item == null)
                return RedirectToAction("List");

            var model = item.ToModel<InvoiceRequestModel>();
            // model.TransitCodes = (await _invoiceRequestService.GetTransitCodesByIdRequestAsync(item.Id)).Select(x => x.Code).ToList();
            
            return View(model);
        }

        #region POST METHODS
        
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<ActionResult> Edit(InvoiceRequestModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(PermissionHelper.AccessFileVintageUpload))
                return Content("Access denied");

            if (ModelState.IsValid)
            {
                var item = await _invoiceRequestService.GetByIdAsync(model.Id);
                if (item == null)
                    return RedirectToAction("List");

                item = model.ToEntity(item);
                await _invoiceRequestService.UpdateAsync(item);

                //activity log
                await _customerActivityService.InsertActivityAsync("EditFileVintageUpload",
                    await _localizationService.GetResourceAsync($"{MTB.PLUGIN_NAME_SPACE}.InvoiceRequest.Updated"), item);

                _notificationService.SuccessNotification(
                    await _localizationService.GetResourceAsync($"{MTB.PLUGIN_NAME_SPACE}.InvoiceRequest.Updated"));

                // Generate JSon to send in FTP folder requests
                var codes = await _invoiceRequestService.GetTransitCodesByIdRequestAsync(item.Id);
                model.TransitCodes = codes.Select(x => x.Code).ToList();
                var jsonContent = JsonConvert.SerializeObject(model);

                return continueEditing ? RedirectToAction("Edit", new { id = item.Id }) : RedirectToAction("List");
            }

            return View(model);
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
        
        #endregion
    }
}