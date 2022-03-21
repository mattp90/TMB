using Nop.Core;
using System;
using System.Threading.Tasks;

namespace Nop.Plugin.MTB.Services.InvoiceRequest
{
    public interface IInvoiceRequestService
    {
        Task<IPagedList<Entity.InvoiceRequest>> GetAllAsync(int invoiceRequestId = 0, string searchName = "",
            string searchSurname = "", string searchBusinessName = "",
            string searchFiscalCode = "", string searchPEC = "", string searchTransitCode = "", int pageIndex = 0,
            int pageSize = Int32.MaxValue, bool noCache = false);
        
        Task<Entity.InvoiceRequest> GetByIdAsync(int id);
        
        Task<Entity.InvoiceRequest> GetDetailByIdAsync(int id);
        
        Task InsertAsync(Entity.InvoiceRequest item);
        
        Task UpdateAsync(Entity.InvoiceRequest item);
        
        Task DeleteAsync(Entity.InvoiceRequest item);

        Task InsertTransitCode(Entity.InvoiceRequestTransitCode item);

        Task InsertAddress(Entity.InvoiceRequestAddress item);
    }
}
