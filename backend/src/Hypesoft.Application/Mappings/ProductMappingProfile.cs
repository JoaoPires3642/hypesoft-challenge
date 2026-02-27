using AutoMapper;
using Hypesoft.Application.DTOs;
using Hypesoft.Domain.Entities;

namespace Hypesoft.Application.Mappings;

/// <summary>
/// Profile de mapeamento para entidade Product
/// </summary>
public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        // Product → ProductResponse
        CreateMap<Product, ProductResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.IsStockLow, opt => opt.MapFrom(src => src.IsStockLow()));
    }
}
