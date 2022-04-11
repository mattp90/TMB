using System;
using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.TMB.Entity;
using Nop.Plugin.TMB.Model.Admin.InvoiceRequest;

namespace Nop.Plugin.TMB.Data
{
    public class AdminMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public int Order => int.MaxValue;

        public AdminMapperConfiguration()
        {
            CreateMap<InvoiceRequest, InvoiceRequestModel>()
                .ForMember(
                        dest => dest.InvoiceRequestAddress,
                        opt => opt.MapFrom(src => src.InvoiceRequestAddress))
                
                .ForMember(
                    dest => dest.InvoiceRequestFiscalId,
                    opt => opt.MapFrom(src => src.InvoiceRequestFiscalId));
                
            CreateMap<InvoiceRequestModel, InvoiceRequest>();
            CreateMap<InvoiceRequestAddressModel, InvoiceRequestAddress>();
            CreateMap<InvoiceRequestAddress, InvoiceRequestAddressModel>();
            CreateMap<InvoiceRequest, InvoiceRequestForGridModel>()
                .ForMember(
                dest => dest.RequestDate,
                opt => opt.MapFrom(src =>
                    src.RequestDate.HasValue ? src.RequestDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty))
                .ForMember(
                    dest => dest.ResponseDate,
                    opt => opt.MapFrom(src =>
                        src.ResponseDate.HasValue ? src.ResponseDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty));
            CreateMap<InvoiceRequestTransitCode, InvoiceRequestTransitCodeModel>();
            CreateMap<InvoiceRequestFiscalId, InvoiceRequestFiscalIdModel>();
            CreateMap<InvoiceRequestFiscalIdModel, InvoiceRequestFiscalId>();
        }
    }
}
