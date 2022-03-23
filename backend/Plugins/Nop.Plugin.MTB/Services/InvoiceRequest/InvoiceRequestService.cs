using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using iTextSharp.text.pdf;
using Nop.Plugin.MTB.Entity;
using Org.BouncyCastle.Ocsp;

namespace Nop.Plugin.MTB.Services.InvoiceRequest
{
    public class InvoiceRequestService : IInvoiceRequestService
    {
        private readonly IRepository<Entity.InvoiceRequest> _invoiceRequestRepository;
        private readonly IRepository<InvoiceRequestAddress> _invoiceRequestAddressRepository;
        private readonly IRepository<InvoiceRequestState> _invoiceRequestStateRepository;
        private readonly IRepository<InvoiceRequestTransitCode> _invoiceRequestTransitCodeRepository;
        private readonly IRepository<InvoiceRequestTransitCodeState> _invoiceRequestTransitCodeStateRepository;
        private readonly IStaticCacheManager _cacheManager;

        private const string InvoiceRequestItemAll = MTB.PLUGIN_NAME_SPACE + ".InvoiceRequest.all";

        public InvoiceRequestService(IRepository<Entity.InvoiceRequest> invoiceRequestRepository, IStaticCacheManager cacheManager, 
            IRepository<InvoiceRequestAddress> invoiceRequestAddressRepository, IRepository<InvoiceRequestState> invoiceRequestStateRepository, IRepository<InvoiceRequestTransitCode> invoiceRequestTransitCodeRepository, IRepository<InvoiceRequestTransitCodeState> invoiceRequestTransitCodeStateRepository)
        {
            _invoiceRequestRepository = invoiceRequestRepository;
            _cacheManager = cacheManager;
            _invoiceRequestAddressRepository = invoiceRequestAddressRepository;
            _invoiceRequestStateRepository = invoiceRequestStateRepository;
            _invoiceRequestTransitCodeRepository = invoiceRequestTransitCodeRepository;
            _invoiceRequestTransitCodeStateRepository = invoiceRequestTransitCodeStateRepository;
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
        
        public virtual async Task<Entity.InvoiceRequest> GetDetailByIdAsync(int id)
        {
            return await GetInvoiceRequestDetail(id);
        }
        
        public virtual async Task<List<InvoiceRequestTransitCode>> GetTransitionCodesByIdRequestAsync(int id)
        {
            var codes = from t in _invoiceRequestTransitCodeRepository.Table
                where t.InvoiceRequestId == id
                select t;

            return await codes.ToListAsync();
        }
        
        public virtual async System.Threading.Tasks.Task InsertAsync(Entity.InvoiceRequest item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.CreatedOnUtc = DateTime.Now;
            item.UpdatedOnUtc = DateTime.Now;

            await _invoiceRequestRepository.InsertAsync(item);
        }

        public virtual async System.Threading.Tasks.Task UpdateAsync(Entity.InvoiceRequest item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            
            item.UpdatedOnUtc = DateTime.Now;

            await _invoiceRequestRepository.UpdateAsync(item);
        }

        public virtual async System.Threading.Tasks.Task DeleteAsync(Entity.InvoiceRequest item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            
            await _invoiceRequestRepository.DeleteAsync(item);
        }
        
        public virtual async System.Threading.Tasks.Task InsertAddressAsync(InvoiceRequestAddress item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.CreatedOnUtc = DateTime.Now;
            item.UpdatedOnUtc = DateTime.Now;
            
            await _invoiceRequestAddressRepository.InsertAsync(item);
        }
        
        public virtual async System.Threading.Tasks.Task InsertTransitCode(InvoiceRequestTransitCode item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            
            item.CreatedOnUtc = DateTime.Now;
            item.UpdatedOnUtc = DateTime.Now;

            await _invoiceRequestTransitCodeRepository.InsertAsync(item);
        }
        
        #region PRIVATE METHODS 
        
        private async Task<IPagedList<Entity.InvoiceRequest>> GetItems(int invoiceRequestId = 0, string searchName = "",  string searchSurname = "", string searchBusinessName = "",
            string searchFiscalCode = "", string searchPEC = "", string searchTransitCode = "", int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            var query = from p in _invoiceRequestRepository.Table
                join state in _invoiceRequestStateRepository.Table  on p.InvoiceRequestStateId equals state.Id
                select new Entity.InvoiceRequest()
                {
                    Id = p.Id,
                    Deleted = p.Deleted,
                    Name = p.Name,
                    Surname = p.Surname,
                    BusinessName = p.BusinessName,
                    FiscalCode = p.FiscalCode,
                    LastUpdate = p.LastUpdate,
                    RequestDate = p.RequestDate,
                    ResponseDate = p.ResponseDate,
                    CreatedOnUtc = p.CreatedOnUtc,
                    PEC = p.PEC,
                    UpdatedOnUtc = p.UpdatedOnUtc,
                    InvoiceRequestStateId = p.InvoiceRequestStateId,
                    InvoiceRequestState = state
                };

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
            
            return await query.OrderBy(m => m.Id).ThenBy(x => x.Name).ToPagedListAsync(pageIndex-1, pageSize);
        }

        private async Task<Entity.InvoiceRequest> GetInvoiceRequestDetail(int id)
        {
            var query = from r in _invoiceRequestRepository.Table
                join state in _invoiceRequestStateRepository.Table on r.InvoiceRequestStateId equals state.Id
                where r.Id == id
                select new Entity.InvoiceRequest()
                {   
                    Id = r.Id,
                    Deleted = r.Deleted,
                    Name = r.Name,
                    Surname = r.Surname,
                    BusinessName = r.BusinessName,
                    FiscalCode = r.FiscalCode,
                    LastUpdate = r.LastUpdate,
                    RequestDate = r.RequestDate,
                    ResponseDate = r.ResponseDate,
                    CreatedOnUtc = r.CreatedOnUtc,
                    PEC = r.PEC,
                    UpdatedOnUtc = r.UpdatedOnUtc,
                    InvoiceRequestStateId = r.InvoiceRequestStateId,
                    InvoiceRequestState = state
                };

            var invoiceRequest = query.FirstOrDefault();

            if (invoiceRequest != null)
            {
                invoiceRequest.InvoiceRequestAddress = await (from a in _invoiceRequestAddressRepository.Table
                    where a.InvoiceRequestId == invoiceRequest.Id
                    select a).FirstOrDefaultAsync();

                invoiceRequest.InvoiceRequestTransitCode = await (from c in _invoiceRequestTransitCodeRepository.Table
                    where c.InvoiceRequestId == invoiceRequest.Id
                    select new InvoiceRequestTransitCode()
                    {
                        Id = c.Id,
                        Code = c.Code,
                        PdfName = c.PdfName,
                        CreatedOnUtc = c.CreatedOnUtc,
                        InvoiceRequestId = c.InvoiceRequestId,
                        UpdatedOnUtc = c.UpdatedOnUtc,
                        InvoiceRequestTransitionCodeStateId = c.InvoiceRequestTransitionCodeStateId,
                        // InvoiceRequestTransitionCodeState = state
                    }).ToListAsync();

                return invoiceRequest;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
