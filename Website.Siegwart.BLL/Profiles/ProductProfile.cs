using AutoMapper;
using Website.Siegwart.DAL.Models;
using Website.Siegwart.BLL.Dtos.Admin.ProductDtos;
using Website.Siegwart.BLL.Dtos.User;

namespace Website.Siegwart.BLL.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        // ========== Admin Mappings ==========

        // Product → ProductListDto
        CreateMap<Product, ProductListDto>()
            .ForMember(dest => dest.CategoryNameEn,
                opt => opt.MapFrom(src => src.Category.NameEn))
            .ForMember(dest => dest.CategoryNameAr,
                opt => opt.MapFrom(src => src.Category.NameAr));

        // Product → ProductDetailsDto
        CreateMap<Product, ProductDetailsDto>()
            .ForMember(dest => dest.CategoryNameEn,
                opt => opt.MapFrom(src => src.Category.NameEn))
            .ForMember(dest => dest.CategoryNameAr,
                opt => opt.MapFrom(src => src.Category.NameAr));

        // CreateProductDto → Product
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifiedOn, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Slug, opt => opt.Ignore())
            .ForMember(dest => dest.OgImageUrl, opt => opt.Ignore());

        // UpdateProductDto → Product
        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifiedOn, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Slug, opt => opt.Ignore())
            .ForMember(dest => dest.OgImageUrl, opt => opt.Ignore());

        // Product → UpdateProductDto (for edit form)
        CreateMap<Product, UpdateProductDto>();

        // ========== User (Frontend) Mappings ==========

        // Product → UserProductDto
        CreateMap<Product, UserProductDto>()
            .ForMember(dest => dest.CategoryNameEn,
                opt => opt.MapFrom(src => src.Category.NameEn))
            .ForMember(dest => dest.CategoryNameAr,
                opt => opt.MapFrom(src => src.Category.NameAr));
    }
}