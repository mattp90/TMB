﻿using Nop.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.MTB.Entity;

namespace Nop.Plugin.MTB.Services.InvoiceRequest
{
    public interface IInvoiceRequestService
    {
        Task<IPagedList<Entity.InvoiceRequest>> GetAllAsync(int invoiceRequestId = 0, string searchName = "",
            string searchSurname = "", string searchBusinessName = "",
            string searchFiscalCode = "", string searchPEC = "", string searchTransitCode = "", int pageIndex = 0,
            int pageSize = Int32.MaxValue, bool noCache = false);
        
        Task<Entity.InvoiceRequest> GetByIdAsync(int id);

        Task<Entity.InvoiceRequest> GetByGuidAsync(Guid guid);
        
        Task<IPagedList<InvoiceRequestTransitCode>> GetTransitCodesByIdRequestAsync(int id);

        Task<InvoiceRequestTransitCode> GetTransitCodesByRequestIdAndCode(int id, string code);
        
        Task<Entity.InvoiceRequest> GetDetailByIdAsync(int id);

        Task<Entity.InvoiceRequest> GetDetailByGuidAsync(string guid);

        System.Threading.Tasks.Task InsertAsync(Entity.InvoiceRequest item);

        System.Threading.Tasks.Task InsertAddressAsync(InvoiceRequestAddress item);
        
        System.Threading.Tasks.Task InsertFiscalIdAsync(InvoiceRequestFiscalId item);
        
        System.Threading.Tasks.Task InsertTransitCode(InvoiceRequestTransitCode item);

        System.Threading.Tasks.Task UpdateAsync(Entity.InvoiceRequest item);

        System.Threading.Tasks.Task UpdateTransitCodeAsync(InvoiceRequestTransitCode item);
        
        System.Threading.Tasks.Task DeleteAsync(Entity.InvoiceRequest item);
    }
}
