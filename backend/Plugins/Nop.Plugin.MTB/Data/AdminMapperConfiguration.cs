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
                .ForMember(
                        dest => dest.Address,
                        opt => opt.MapFrom(src => src.InvoiceRequestAddress))
                .ForMember(
                    dest => dest.TransitCodesList,
                    opt => opt.MapFrom(src => src.InvoiceRequestTransitCode));
                
                CreateMap<InvoiceRequestModel, InvoiceRequest>();
                //.ForMember(
                //    dest => dest.BusinessName,
                //    opt => opt.MapFrom(src => src.Business_name));
            
            CreateMap<InvoiceRequestAddressModel, InvoiceRequestAddress>();
            CreateMap<InvoiceRequestAddress, InvoiceRequestAddressModel>();
            CreateMap<InvoiceRequest, InvoiceRequestForGridModel>();
            CreateMap<InvoiceRequestTransitCode, InvoiceRequestTransitCodeModel>();
        }
    }
}
