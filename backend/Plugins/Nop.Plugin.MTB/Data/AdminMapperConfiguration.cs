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
            CreateMap<InvoiceRequest, InvoiceRequestModel>()
                .ForMember(// .ForMember(
                                           //     dest => dest.TransitCodesList,
                                           //     opt => opt.MapFrom(src => src.InvoiceRequestTransitCode))
                        dest => dest.InvoiceRequestAddress,
                        opt => opt.MapFrom(src => src.InvoiceRequestAddress))
                
                .ForMember(
                    dest => dest.InvoiceRequestFiscalId,
                    opt => opt.MapFrom(src => src.InvoiceRequestFiscalId));
                
            CreateMap<InvoiceRequestModel, InvoiceRequest>();
            CreateMap<InvoiceRequestAddressModel, InvoiceRequestAddress>();
            CreateMap<InvoiceRequestAddress, InvoiceRequestAddressModel>();
            CreateMap<InvoiceRequest, InvoiceRequestForGridModel>();
            CreateMap<InvoiceRequestTransitCode, InvoiceRequestTransitCodeModel>();
            CreateMap<InvoiceRequestFiscalId, InvoiceRequestFiscalIdModel>();
            CreateMap<InvoiceRequestFiscalIdModel, InvoiceRequestFiscalId>();
        }
    }
}
