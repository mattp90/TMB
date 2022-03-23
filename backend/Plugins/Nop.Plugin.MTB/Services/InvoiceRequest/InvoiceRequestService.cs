using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Nop.Plugin.MTB.Entity;


namespace Nop.Plugin.MTB.Services.InvoiceRequest
{
    public class InvoiceRequestService : IInvoiceRequestService
    {
        private readonly IRepository<Entity.InvoiceRequest> _invoiceRequestRepository;
        private readonly IRepository<InvoiceRequestTransitCode> _transitionCodeRepository;
        private readonly IRepository<InvoiceRequestAddress> _invoiceRequestAddressRepository;
        private readonly IStaticCacheManager _cacheManager;

        private const string InvoiceRequestItemAll = MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.all";

        public InvoiceRequestService(IRepository<Entity.InvoiceRequest> invoiceRequestRepository, IStaticCacheManager cacheManager, 
            IRepository<InvoiceRequestTransitCode> transitionCodeRepository, IRepository<InvoiceRequestAddress> invoiceRequestAddressRepository)
        {
            _invoiceRequestRepository = invoiceRequestRepository;
            _cacheManager = cacheManager;
            _transitionCodeRepository = transitionCodeRepository;
            _invoiceRequestAddressRepository = invoiceRequestAddressRepository;
        }

        public async Task<IPagedList<Entity.InvoiceRequest>> GetAllAsync(int invoiceRequestId = 0, string searchName = "", string searchSurname = "", string searchBusinessName = "",
            string searchFiscalCode = "", string searchPEC = "", string searchTransitCode = "", int pageIndex = 0,
            int pageSize = Int32.MaxValue, bool noCache = false)
        {
            IPagedList<Entity.InvoiceRequest> items;

            if (noCache)
            {
                items = await GetItems(invoiceRequestId, searchName, searchSurname, searchBusinessName, searchFiscalCode,
                    searchPEC, searchTransitCode, pageIndex, pageSize);
            }
            else
            {
                var cacheKey = InvoiceRequestItemAll;
                items = await _cacheManager.GetAsync(new CacheKey(cacheKey),
                    async () => await GetItems(invoiceRequestId, searchName, searchSurname, searchBusinessName, searchFiscalCode,
                        searchPEC, searchTransitCode, pageIndex, pageSize));
            }

            return  new PagedList<Entity.InvoiceRequest>(items, pageIndex - 1, pageSize);
        }

        public virtual async Task<Entity.InvoiceRequest> GetByIdAsync(int id)
        {
            return await _invoiceRequestRepository.GetByIdAsync(id, cache => default);
        }
        
        public virtual async Task<InvoiceRequestAddress> GetAddressByIdRequestAsync(int id)
        {
            var address = from a in _invoiceRequestAddressRepository.Table
                where a.InvoiceRequestId == id
                select a;

            return await address.FirstOrDefaultAsync();
        }
        
        public virtual async Task<List<InvoiceRequestTransitCode>> GetTransitionCodesByIdRequestAsync(int id)
        {
            var codes = from t in _transitionCodeRepository.Table
                where t.InvoiceRequestId == id
                select t;

            return await codes.ToListAsync();
        }

        public virtual Task<Entity.InvoiceRequest> GetDetailByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        
        public virtual async Task InsertAsync(Entity.InvoiceRequest item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.CreatedOnUTC = DateTime.Now;
            item.UpdatedOnUTC = DateTime.Now;

            await _invoiceRequestRepository.InsertAsync(item);
        }

        public virtual async Task UpdateAsync(Entity.InvoiceRequest item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            
            item.UpdatedOnUTC = DateTime.Now;

            await _invoiceRequestRepository.UpdateAsync(item);
        }

        public virtual async Task DeleteAsync(Entity.InvoiceRequest item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            
            await _invoiceRequestRepository.DeleteAsync(item);
        }
        
        public virtual async Task InsertAddress(InvoiceRequestAddress item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            await _invoiceRequestAddressRepository.InsertAsync(item);
        }
        
        public virtual async Task InsertTransitCode(InvoiceRequestTransitCode item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            await _transitionCodeRepository.InsertAsync(item);
        }
        
        #region PRIVATE METHODS 
        
        private async Task<IPagedList<Entity.InvoiceRequest>> GetItems(int invoiceRequestId = 0, string searchName = "",  string searchSurname = "", string searchBusinessName = "",
            string searchFiscalCode = "", string searchPEC = "", string searchTransitCode = "", int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            var query = from p in _invoiceRequestRepository.Table
                select p;

            if (invoiceRequestId > 0)
            {
                query = from p in query 
                    where p.Id == invoiceRequestId
                    select p;
            }

            if (!string.IsNullOrEmpty(searchName))
            {
                query = from p in query
                    where p.Name.Contains(searchName)
                    select p;
            }

            if (!string.IsNullOrEmpty(searchSurname))
            {
                query = from p in query
                    where p.Surname.Contains(searchSurname)
                    select p;
            }
            
            if (!string.IsNullOrEmpty(searchBusinessName))
            {
                query = from p in query
                    where p.BusinessName.Contains(searchBusinessName)
                    select p;
            }

            if (!string.IsNullOrEmpty(searchFiscalCode))
            {
                query = from p in query
                    where p.FiscalCode.Contains(searchFiscalCode)
                    select p;
            }

            if (!string.IsNullOrEmpty(searchPEC))
            {
                query = from p in query
                    where p.PEC.Contains(searchPEC)
                    select p;
            }

            // if (!string.IsNullOrEmpty(searchTransitCode))
            // {
            //     query = from p in query
            //         where p..Contains(searchBusinessName)
            //         select p;
            // }
            
            return await query.OrderBy(m => m.Id).OrderBy(x => x.Name).ToPagedListAsync(pageIndex-1, pageSize);
        }
        
        #endregion
    }
}
