using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using FluentMigrator.Runner.Processors.SqlServer;
using iTextSharp.text.pdf;
using Nop.Plugin.TMB.Entity;
using Org.BouncyCastle.Ocsp;

namespace Nop.Plugin.TMB.Services.InvoiceRequest
{
    public class InvoiceRequestService : IInvoiceRequestService
    {
        private readonly IRepository<Entity.InvoiceRequest> _invoiceRequestRepository;
        private readonly IRepository<InvoiceRequestAddress> _invoiceRequestAddressRepository;
        private readonly IRepository<InvoiceRequestState> _invoiceRequestStateRepository;
        private readonly IRepository<InvoiceRequestTransitCode> _invoiceRequestTransitCodeRepository;
        private readonly IRepository<InvoiceRequestTransitCodeState> _invoiceRequestTransitCodeStateRepository;
        private readonly IRepository<InvoiceRequestFiscalId> _invoiceRequestFiscalIdRepository;
        private readonly IStaticCacheManager _cacheManager;

        private const string InvoiceRequestItemAll = TMB.PLUGIN_NAME_SPACE + ".InvoiceRequest.all";

        public InvoiceRequestService(IRepository<Entity.InvoiceRequest> invoiceRequestRepository, IStaticCacheManager cacheManager, 
            IRepository<InvoiceRequestAddress> invoiceRequestAddressRepository, IRepository<InvoiceRequestState> invoiceRequestStateRepository, IRepository<InvoiceRequestTransitCode> invoiceRequestTransitCodeRepository, IRepository<InvoiceRequestTransitCodeState> invoiceRequestTransitCodeStateRepository, IRepository<InvoiceRequestFiscalId> invoiceRequestFiscalIdRepository)
        {
            _invoiceRequestRepository = invoiceRequestRepository;
            _cacheManager = cacheManager;
            _invoiceRequestAddressRepository = invoiceRequestAddressRepository;
            _invoiceRequestStateRepository = invoiceRequestStateRepository;
            _invoiceRequestTransitCodeRepository = invoiceRequestTransitCodeRepository;
            _invoiceRequestTransitCodeStateRepository = invoiceRequestTransitCodeStateRepository;
            _invoiceRequestFiscalIdRepository = invoiceRequestFiscalIdRepository;
        }

        public async Task<IPagedList<Entity.InvoiceRequest>> GetAllAsync(int invoiceRequestId = 0, string searchName = "", string searchSurname = "", string searchBusinessName = "",
            string searchFiscalCode = "", string searchPEC = "", string searchTransitCode = "", int searchStateId = 0, int pageIndex = 0,
            int pageSize = Int32.MaxValue, bool noCache = false)
        {
            IQueryable<Entity.InvoiceRequest> queryInvoiceRequest;

            if (noCache)
            {
                queryInvoiceRequest = GetItems(invoiceRequestId, searchName, searchSurname, searchBusinessName, searchFiscalCode,
                    searchPEC, searchTransitCode, searchStateId, pageIndex, pageSize);
            }
            else
            {
                var cacheKey = InvoiceRequestItemAll;
                queryInvoiceRequest = _cacheManager.Get(new CacheKey(cacheKey),
                    () => GetItems(invoiceRequestId, searchName, searchSurname, searchBusinessName, searchFiscalCode,
                        searchPEC, searchTransitCode, searchStateId, pageIndex, pageSize));
            }

            return await queryInvoiceRequest.ToPagedListAsync(pageIndex - 1, pageSize);
        }

        public virtual async Task<Entity.InvoiceRequest> GetByIdAsync(int id)
        {
            return await _invoiceRequestRepository.GetByIdAsync(id, cache => default);
        }
        
        public virtual async Task<Entity.InvoiceRequest> GetByGuidAsync(Guid guid)
        {
            var item = from r in _invoiceRequestRepository.Table
                where r.GuidId == guid
                select r;

            return await item.FirstOrDefaultAsync();
        }
        
        public virtual async Task<Entity.InvoiceRequest> GetDetailByIdAsync(int id)
        {
            return await GetInvoiceRequestDetail(id);
        }
        
        public virtual async Task<Entity.InvoiceRequest> GetDetailByGuidAsync(string guid)
        {
            return await GetInvoiceRequestDetailByGuid(guid);
        }

        public virtual async Task<IPagedList<InvoiceRequestTransitCode>> GetTransitCodesByIdRequestAsync(int id)
        {
            var codes = from t in _invoiceRequestTransitCodeRepository.Table
                join states in _invoiceRequestTransitCodeStateRepository.Table on t.InvoiceRequestTransitCodeStateId equals states.Id into CodeStates
                from state in CodeStates.DefaultIfEmpty()
                where t.InvoiceRequestId == id
                select new InvoiceRequestTransitCode()
                {
                    Id = t.Id,
                    Code = t.Code,
                    PdfName = t.PdfName,
                    CreatedOnUtc = t.CreatedOnUtc,
                    InvoiceRequestId = t.InvoiceRequestId,
                    UpdatedOnUtc = t.UpdatedOnUtc,
                    InvoiceRequestTransitCodeStateId = t.InvoiceRequestTransitCodeStateId,
                    InvoiceRequestTransitCodeState = state
                };

            // non ho cazzi di fare la paginazione, tanto non ci saranno richieste con 800mila codici
            return await codes.ToPagedListAsync(0, Int32.MaxValue);
        }

        public virtual IList<InvoiceRequestState> GetInvoiceRequestStates()
        {
            return _invoiceRequestStateRepository.GetAll();
        }

        public virtual async Task<InvoiceRequestTransitCode> GetTransitCodesByRequestIdAndCode(int id, string code)
        {
            return await (from t in _invoiceRequestTransitCodeRepository.Table
                where (t.InvoiceRequestId == id && t.Code == code) 
                select t).FirstOrDefaultAsync();
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

        public virtual async System.Threading.Tasks.Task InsertStateAsync(InvoiceRequestState item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.CreatedOnUtc = DateTime.Now;
            item.UpdatedOnUtc = DateTime.Now;

            await _invoiceRequestStateRepository.InsertAsync(item);
        }
        
        public virtual async System.Threading.Tasks.Task InsertTransitionCodeStateAsync(InvoiceRequestTransitCodeState item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.CreatedOnUtc = DateTime.Now;
            item.UpdatedOnUtc = DateTime.Now;

            await _invoiceRequestTransitCodeStateRepository.InsertAsync(item);
        }
        
        public virtual async System.Threading.Tasks.Task UpdateAsync(Entity.InvoiceRequest item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            
            item.UpdatedOnUtc = DateTime.Now;

            await _invoiceRequestRepository.UpdateAsync(item);

            if (item.InvoiceRequestAddress != null)
            {
                item.InvoiceRequestAddress.UpdatedOnUtc = DateTime.Now;
                await _invoiceRequestAddressRepository.UpdateAsync(item.InvoiceRequestAddress);    
            }

            if (item.InvoiceRequestFiscalId != null)
            {
                item.InvoiceRequestFiscalId.UpdatedOnUtc = DateTime.Now;
                await _invoiceRequestFiscalIdRepository.UpdateAsync(item.InvoiceRequestFiscalId);    
            }
        }
        
        public virtual async System.Threading.Tasks.Task UpdateTransitCodeAsync(InvoiceRequestTransitCode item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            
            item.UpdatedOnUtc = DateTime.Now;

            await _invoiceRequestTransitCodeRepository.UpdateAsync(item);
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
        
        public virtual async System.Threading.Tasks.Task InsertFiscalIdAsync(InvoiceRequestFiscalId item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.CreatedOnUtc = DateTime.Now;
            item.UpdatedOnUtc = DateTime.Now;
            
            await _invoiceRequestFiscalIdRepository.InsertAsync(item);
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
        
        private IQueryable<Entity.InvoiceRequest> GetItems(int invoiceRequestId = 0, string searchName = "",  string searchSurname = "", string searchBusinessName = "",
            string searchFiscalCode = "", string searchPEC = "", string searchTransitCode = "", int searchStateId = 0, int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            var query = from p in _invoiceRequestRepository.Table
                join states in _invoiceRequestStateRepository.Table  on p.InvoiceRequestStateId equals states.Id 
                into StatesOfReq
                from state in StatesOfReq.DefaultIfEmpty()
                select new Entity.InvoiceRequest()
                {
                    GuidId = p.GuidId,
                    Id = p.Id,
                    Deleted = p.Deleted,
                    Name = p.Name,
                    Surname = p.Surname,
                    BusinessName = p.BusinessName,
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
                var idList = (from f in _invoiceRequestFiscalIdRepository.Table
                    where (f.Code == searchTransitCode || f.VatCode == searchFiscalCode) 
                    select f.InvoiceRequestId).ToList();
                
                query = from p in query
                    where idList.Contains(p.Id)
                    select p;
            }

            if (!string.IsNullOrEmpty(searchPEC))
            {
                query = from p in query
                    where p.PEC.Contains(searchPEC)
                    select p;
            }

            if (!string.IsNullOrEmpty(searchTransitCode))
            {
                var idList = (from c in _invoiceRequestTransitCodeRepository.Table
                    where c.Code == searchTransitCode
                    select c.InvoiceRequestId).ToList();
                
                query = from p in query
                    where idList.Contains(p.Id)
                    select p;
            }
            
            if (searchStateId > 0)
            {
                query = from p in query
                    where p.InvoiceRequestState.Id == searchStateId
                    select p;
            }
            
            return query.OrderByDescending(m => m.RequestDate);
        }   

        private async Task<Entity.InvoiceRequest> GetInvoiceRequestDetail(int id)
        {
            var query = from r in _invoiceRequestRepository.Table
                join states in _invoiceRequestStateRepository.Table  on r.InvoiceRequestStateId equals states.Id 
                into StatesOfReq
                from state in StatesOfReq.DefaultIfEmpty()
                join fc in _invoiceRequestFiscalIdRepository.Table on r.Id equals fc.InvoiceRequestId into FiscalCodes
                from fcode in FiscalCodes.DefaultIfEmpty()
                where r.Id == id
                select new Entity.InvoiceRequest()
                {   
                    Id = r.Id,
                    GuidId = r.GuidId,
                    Deleted = r.Deleted,
                    Name = r.Name,
                    Surname = r.Surname,
                    BusinessName = r.BusinessName,
                    SdICode = r.SdICode,
                    InvoiceRequestFiscalId = fcode,
                    LastUpdate = r.LastUpdate,
                    RequestDate = r.RequestDate,
                    ResponseDate = r.ResponseDate,
                    PEC = r.PEC,
                    InvoiceRequestStateId = r.InvoiceRequestStateId,
                    InvoiceRequestState = state,
                    CreatedOnUtc = r.CreatedOnUtc,
                    UpdatedOnUtc = r.UpdatedOnUtc
                };

            var invoiceRequest = query.FirstOrDefault();

            if (invoiceRequest != null)
            {
                invoiceRequest.InvoiceRequestAddress = await (from a in _invoiceRequestAddressRepository.Table
                    where a.InvoiceRequestId == invoiceRequest.Id
                    select a).FirstOrDefaultAsync();

                // invoiceRequest.InvoiceRequestTransitCode = await (from c in _invoiceRequestTransitCodeRepository.Table
                //     join states in _invoiceRequestTransitCodeStateRepository.Table on c.InvoiceRequestTransitCodeStateId equals states.Id into CodeStates
                //     from state in CodeStates.DefaultIfEmpty()
                //     where c.InvoiceRequestId == invoiceRequest.Id
                //     select new InvoiceRequestTransitCode()
                //     {
                //         Id = c.Id,
                //         Code = c.Code,
                //         PdfName = c.PdfName,
                //         CreatedOnUtc = c.CreatedOnUtc,
                //         InvoiceRequestId = c.InvoiceRequestId,
                //         UpdatedOnUtc = c.UpdatedOnUtc,
                //         InvoiceRequestTransitCodeStateId = c.InvoiceRequestTransitCodeStateId,
                //         InvoiceRequestTransitCodeState = state
                //     }).ToListAsync();
                
                return invoiceRequest;
            }
            else
            {
                return null;
            }
        }
        
        private async Task<Entity.InvoiceRequest> GetInvoiceRequestDetailByGuid(string guid)
        {
            var query = from r in _invoiceRequestRepository.Table
                join states in _invoiceRequestStateRepository.Table  on r.InvoiceRequestStateId equals states.Id 
                into StatesOfReq
                from state in StatesOfReq.DefaultIfEmpty()
                join fc in _invoiceRequestFiscalIdRepository.Table on r.Id equals fc.InvoiceRequestId into FiscalCodes
                from fcode in FiscalCodes.DefaultIfEmpty()
                where r.GuidId == new Guid(guid)
                select new Entity.InvoiceRequest()
                {   
                    Id = r.Id,
                    GuidId = r.GuidId,
                    Deleted = r.Deleted,
                    Name = r.Name,
                    Surname = r.Surname,
                    BusinessName = r.BusinessName,
                    SdICode = r.SdICode,
                    InvoiceRequestFiscalId = fcode,
                    LastUpdate = r.LastUpdate,
                    RequestDate = r.RequestDate,
                    ResponseDate = r.ResponseDate,
                    PEC = r.PEC,
                    InvoiceRequestStateId = r.InvoiceRequestStateId,
                    InvoiceRequestState = state,
                    CreatedOnUtc = r.CreatedOnUtc,
                    UpdatedOnUtc = r.UpdatedOnUtc
                };

            var invoiceRequest = query.FirstOrDefault();

            if (invoiceRequest != null)
            {
                invoiceRequest.InvoiceRequestAddress = await (from a in _invoiceRequestAddressRepository.Table
                    where a.InvoiceRequestId == invoiceRequest.Id
                    select a).FirstOrDefaultAsync();
                
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
