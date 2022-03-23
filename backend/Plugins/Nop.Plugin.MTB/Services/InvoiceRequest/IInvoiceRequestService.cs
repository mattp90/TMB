using Nop.Core;
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
        
        Task<IPagedList<InvoiceRequestTransitCode>> GetTransitionCodesByIdRequestAsync(int id);
        
        Task<Entity.InvoiceRequest> GetDetailByIdAsync(int id);
        
        System.Threading.Tasks.Task InsertAsync(Entity.InvoiceRequest item);
        
        System.Threading.Tasks.Task UpdateAsync(Entity.InvoiceRequest item);
        
        System.Threading.Tasks.Task DeleteAsync(Entity.InvoiceRequest item);

        System.Threading.Tasks.Task InsertTransitCode(InvoiceRequestTransitCode item);

        System.Threading.Tasks.Task InsertAddressAsync(InvoiceRequestAddress item);
    }
}
