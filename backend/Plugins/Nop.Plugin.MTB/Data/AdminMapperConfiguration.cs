using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.MTB.Entity;
using Nop.Plugin.MTB.Model.Admin.InvoiceRequest;

namespace Nop.Plugin.MTB.Data
{
    public class AdminMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public int Order => int.MaxValue;

        public AdminMapperConfiguration()
        {
            CreateMap<InvoiceRequest, InvoiceRequestModel>();
            CreateMap<InvoiceRequestModel, InvoiceRequest>();
            CreateMap<InvoiceRequestAddressModel, InvoiceRequestAddress>();
            CreateMap<InvoiceRequestAddress, InvoiceRequestAddressModel>();
            CreateMap<InvoiceRequest, InvoiceRequestForGridModel>();
        }
    }
}
