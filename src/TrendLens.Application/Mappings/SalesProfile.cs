using AutoMapper;
using TrendLens.Core.DTOs;
using TrendLens.Core.Entities;

namespace TrendLens.Application.Mappings;

public class SalesProfile : Profile
{
    public SalesProfile()
    {
        CreateMap<SalesRecord, SalesRecordDto>()
            .ForMember(dest => dest.TotalValue, opt => opt.MapFrom(src => src.Amount * src.Quantity))
            .ForMember(dest => dest.DateFormatted, opt => opt.MapFrom(src => src.Date.ToString("yyyy-MM-dd")));

        CreateMap<CreateSalesRecordDto, SalesRecord>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<UpdateSalesRecordDto, SalesRecord>();

        CreateMap<SalesRecordDto, SalesRecord>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
