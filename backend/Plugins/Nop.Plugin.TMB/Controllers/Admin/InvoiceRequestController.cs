using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.TMB.Helpers;
using Nop.Plugin.TMB.Model.Admin.InvoiceRequest;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core.Configuration;
using Nop.Plugin.TMB.Extensions;
using Nop.Plugin.TMB.Services.InvoiceRequest;
using Nop.Services.Localization;
using NopLogger = Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Nop.Core;
using Nop.Core.Domain.Messages;

namespace Nop.Plugin.TMB.Controllers.Admin
{
    public class InvoiceRequestController : BaseAdminController
    {
        private readonly AppSettings _appSettings;
        private readonly IPermissionService _permissionService;
        private readonly IInvoiceRequestService _invoiceRequestService;
        private readonly NopLogger.ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly NopLogger.ILogger _nopLogger;
        private readonly ILogger<InvoiceRequestController> _logger;
        private readonly IEmailAccountService _emailAccountService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ITokenizer _tokenizer;

        public InvoiceRequestController(AppSettings appSettings, IPermissionService permissionService, IInvoiceRequestService invoiceRequestService, NopLogger.ICustomerActivityService customerActivityService, 
            ILocalizationService localizationService, INotificationService notificationService, NopLogger.ILogger nopLogger, ILogger<InvoiceRequestController> logger, IEmailAccountService emailAccountService, EmailAccountSettings emailAccountSettings, IMessageTemplateService messageTemplateService, IQueuedEmailService queuedEmailService, ITokenizer tokenizer)
        {
            _permissionService = permissionService;            
            _invoiceRequestService = invoiceRequestService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _logger = logger;
            _emailAccountService = emailAccountService;
            _emailAccountSettings = emailAccountSettings;
            _messageTemplateService = messageTemplateService;
            _queuedEmailService = queuedEmailService;
            _tokenizer = tokenizer;
            _nopLogger = nopLogger;
            _appSettings = appSettings;
        }

        public virtual IActionResult Index()
        {
            _logger.LogInformation("Enter in InvoiceRequestController");
            return RedirectToAction("List");
        }

        public async Task<ActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(PermissionHelper.AccessInvoiceRequest))
                return new UnauthorizedResult();

            var model = new InvoiceRequestSearchModel();
            PrepareStates(model.SearchStates);
            model.SetGridPageSize();
            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(PermissionHelper.AccessInvoiceRequest))
                return Content("Access denied");

            var item = await _invoiceRequestService.GetDetailByIdAsync(id);
            if (item == null)
                return RedirectToAction("List");

            var model = item.ToModel<InvoiceRequestModel>();
            // model.TransitCodes = (await _invoiceRequestService.GetTransitCodesByIdRequestAsync(item.Id)).Select(x => x.Code).ToList();
            
            return View(model);
        }
        
        #region POST METHODS
        
        [HttpPost]
        public virtual async Task<ActionResult> List(InvoiceRequestSearchModel model)
        {
            if (!await _permissionService.AuthorizeAsync(PermissionHelper.AccessInvoiceRequest))
                return new UnauthorizedResult();

            var invoiceRequestItems = await _invoiceRequestService.GetAllAsync(model.Id, model.SearchName, model.SearchSurname, model.SearchBusinessName, model.SearchFiscalCode, 
                model.SearchPEC, model.SearchTransitCode, model.SearchStatusId, model.Page, model.PageSize, noCache: true);
            
            var gridModel = new InvoiceRequestListModel().PrepareToGrid(model, invoiceRequestItems, () =>
            {
                return invoiceRequestItems.Select(f => f.ToModel<InvoiceRequestForGridModel>());
            });            

            return Json(gridModel);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<ActionResult> Edit(InvoiceRequestModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(PermissionHelper.AccessInvoiceRequest))
                return Content("Access denied");

            if (ModelState.IsValid)
            {
                var item = await _invoiceRequestService.GetByIdAsync(model.Id);
                if (item == null)
                    return RedirectToAction("List");

                item = model.ToEntity(item);
                item.LastUpdate =DateTime.Now;

                try
                {
                    // First send request to FTP; if SUCCESS then save to DB new item
                    // Generate JSon to send in FTP folder requests
                    var codes = await _invoiceRequestService.GetTransitCodesByIdRequestAsync(item.Id);
                    model.TransitCodes = codes.Select(x => x.Code).ToList();
                    var jsonContent = JsonConvert.SerializeObject(model, Formatting.Indented);
                    var ftp = new FTPManager(_appSettings.FtpConfig.Host, _appSettings.FtpConfig.Port,_appSettings.FtpConfig.Username,_appSettings.FtpConfig.Password,_appSettings.FtpConfig.RequestFolder,_appSettings.FtpConfig.ResponseFolder, _appSettings.FtpConfig.ProcessedFolder, _appSettings.FtpConfig.PdfFolder);
                    ftp.UploadFile($"{item.GuidId}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.json", jsonContent);
                    await _invoiceRequestService.UpdateAsync(item);
                        
                    //activity log
                    await _customerActivityService.InsertActivityAsync("EditInvoiceRequest",
                        await _localizationService.GetResourceAsync($"{TMB.PLUGIN_NAME_SPACE}.InvoiceRequest.Updated"), item);

                    _notificationService.SuccessNotification(
                        await _localizationService.GetResourceAsync($"{TMB.PLUGIN_NAME_SPACE}.InvoiceRequest.Updated"));

                }
                catch (Exception e)
                {
                    // print error message
                    await _nopLogger.ErrorAsync($"Error during executing Nop.Plugin.TMB.Controllers.Admin.Edit method for model {model.Id}", e);
                }
                
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
        
        [HttpPost]
        public virtual async Task<ActionResult> ListTransitCode(InvoiceRequestSearchModel model)
        {
            if (!await _permissionService.AuthorizeAsync(PermissionHelper.AccessInvoiceRequest))
                return new UnauthorizedResult();

            var codes = await _invoiceRequestService.GetTransitCodesByIdRequestAsync(model.Id);
            
            var gridModel = new InvoiceRequestTransitCodeListModel().PrepareToGrid(model, codes, () =>
            {
                return codes.Select(f => f.ToModel<InvoiceRequestTransitCodeModel>());
            });            

            return Json(gridModel);
        }

        #endregion
        
        protected virtual void PrepareStates(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available stores
            var states = _invoiceRequestService.GetInvoiceRequestStates();
            foreach (var state in states)
            {
                items.Add(new SelectListItem { Value = state.Id.ToString(), Text = state.Description });
            }
            
            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }
        
        protected virtual async void PrepareDefaultItem(IList<SelectListItem> items, bool withSpecialDefaultItem, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //whether to insert the first special item for the default value
            if (!withSpecialDefaultItem)
                return;

            //at now we use "0" as the default value
            const string regValue = "0";

            //prepare item text
            defaultItemText ??= await _localizationService.GetResourceAsync("Admin.Common.All");

            //insert this default item at first
            items.Insert(0, new SelectListItem { Text = defaultItemText, Value = regValue });
        }

    }
}