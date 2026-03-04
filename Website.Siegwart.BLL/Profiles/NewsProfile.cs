using AutoMapper;
using Website.Siegwart.DAL.Models;
using Website.Siegwart.BLL.Dtos.Admin.NewsDtos;
using Website.Siegwart.BLL.Dtos.User;

namespace Website.Siegwart.BLL.Profiles;

public class NewsProfile : Profile
{
    public NewsProfile()
    {
        // ========== Admin Mappings ==========

        // News → NewsListDto
        CreateMap<News, NewsListDto>();

        // News → NewsDetailsDto
        CreateMap<News, NewsDetailsDto>();

        // CreateNewsDto → News
        CreateMap<CreateNewsDto, News>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifiedOn, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.OgImageUrl, opt => opt.Ignore());

        // UpdateNewsDto → News
        CreateMap<UpdateNewsDto, News>()
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifiedOn, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.OgImageUrl, opt => opt.Ignore());

        // News → UpdateNewsDto (for edit form)
        CreateMap<News, UpdateNewsDto>();

        // ========== User (Frontend) Mappings ==========

        // News → NewsViewDto
        CreateMap<News, NewsViewDto>();
    }
}