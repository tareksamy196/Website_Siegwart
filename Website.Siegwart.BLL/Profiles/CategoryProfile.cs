using AutoMapper;
using Website.Siegwart.DAL.Models;
using Website.Siegwart.BLL.Dtos.Admin.CategoryDtos;
using Website.Siegwart.BLL.Dtos.User;

namespace Website.Siegwart.BLL.Mappings;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        // ========== Admin Mappings ==========

        // Category → CategoryDto (List)
        CreateMap<Category, CategoryDto>();

        // Category → CategoryDetailsDto
        CreateMap<Category, CategoryDetailsDto>()
            .ForMember(dest => dest.ProductCount,
                opt => opt.MapFrom(src => src.Products.Count(p => !p.IsDeleted)));

        // CreateCategoryDto → Category
        CreateMap<CreateCategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifiedOn, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore());

        // UpdateCategoryDto → Category
        CreateMap<UpdateCategoryDto, Category>()
            .ForMember(dest => dest.Products, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifiedOn, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore());

        // Category → UpdateCategoryDto (for edit form)
        CreateMap<Category, UpdateCategoryDto>();

        // CategoryDetailsDto → UpdateCategoryDto (for edit form)
        CreateMap<CategoryDetailsDto, UpdateCategoryDto>();

        // ========== User (Frontend) Mappings ==========

        // Category → UserCategoryDto
        CreateMap<Category, UserCategoryDto>()
            .ForMember(dest => dest.ProductCount,
                opt => opt.MapFrom(src => src.Products.Count(p => !p.IsDeleted && p.IsActive)));
    }
}