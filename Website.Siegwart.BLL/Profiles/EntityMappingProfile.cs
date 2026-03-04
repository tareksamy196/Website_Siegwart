using AutoMapper;
using Website.Siegwart.BLL.Dtos.Admin.NewsDtos;
using Website.Siegwart.BLL.Dtos.Admin.ProductDtos;
using Website.Siegwart.BLL.Dtos.Admin.VideoMedia;
using Website.Siegwart.BLL.Dtos.Admin.TeamMember;
using Website.Siegwart.DAL.Models;

namespace Website.Siegwart.BLL.Profiles
{
    public class EntityMappingProfile : Profile
    {
        public EntityMappingProfile()
        {
            // News
            CreateMap<CreateNewsDto, News>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.SeoKeywords, opt => opt.Ignore())
                .ForMember(d => d.IsDeleted, opt => opt.Ignore())
                .ForMember(d => d.CreatedOn, opt => opt.Ignore())
                .ForMember(d => d.LastModifiedOn, opt => opt.Ignore())
                .ForMember(d => d.CreatedBy, opt => opt.Ignore())
                .ForMember(d => d.LastModifiedBy, opt => opt.Ignore());

            CreateMap<UpdateNewsDto, News>()
                .ForMember(d => d.SeoKeywords, opt => opt.Ignore())
                .ForMember(d => d.IsDeleted, opt => opt.Ignore())
                .ForMember(d => d.CreatedOn, opt => opt.Ignore())
                .ForMember(d => d.LastModifiedOn, opt => opt.Ignore())
                .ForMember(d => d.CreatedBy, opt => opt.Ignore())
                .ForMember(d => d.LastModifiedBy, opt => opt.Ignore());

            CreateMap<News, UpdateNewsDto>()
                .ForMember(d => d.ImageFile, opt => opt.Ignore());

            // Product
            CreateMap<CreateProductDto, Product>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.SeoKeywords, opt => opt.Ignore())
                .ForMember(d => d.IsDeleted, opt => opt.Ignore())
                .ForMember(d => d.CreatedOn, opt => opt.Ignore())
                .ForMember(d => d.LastModifiedOn, opt => opt.Ignore())
                .ForMember(d => d.CreatedBy, opt => opt.Ignore())
                .ForMember(d => d.LastModifiedBy, opt => opt.Ignore());

            CreateMap<UpdateProductDto, Product>()
                .ForMember(d => d.SeoKeywords, opt => opt.Ignore())
                .ForMember(d => d.IsDeleted, opt => opt.Ignore())
                .ForMember(d => d.CreatedOn, opt => opt.Ignore())
                .ForMember(d => d.LastModifiedOn, opt => opt.Ignore())
                .ForMember(d => d.CreatedBy, opt => opt.Ignore())
                .ForMember(d => d.LastModifiedBy, opt => opt.Ignore());

            CreateMap<Product, UpdateProductDto>()
                .ForMember(d => d.ImageFile, opt => opt.Ignore());

            // TeamMember
            // If you have CreateTeamMemberDto, add similar mapping. Here we keep Update mapping.
            CreateMap<TeamMember, UpdateTeamMemberDto>()
                .ForMember(d => d.ImageFile, opt => opt.Ignore());

            // VideoMedia
            CreateMap<VideoMediaCreateDto, VideoMedia>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.IsDeleted, opt => opt.Ignore())
                .ForMember(d => d.CreatedOn, opt => opt.Ignore())
                .ForMember(d => d.LastModifiedOn, opt => opt.Ignore())
                .ForMember(d => d.CreatedBy, opt => opt.Ignore())
                .ForMember(d => d.LastModifiedBy, opt => opt.Ignore());

            CreateMap<VideoMediaUpdateDto, VideoMedia>()
                .ForMember(d => d.IsDeleted, opt => opt.Ignore())
                .ForMember(d => d.CreatedOn, opt => opt.Ignore())
                .ForMember(d => d.LastModifiedOn, opt => opt.Ignore())
                .ForMember(d => d.CreatedBy, opt => opt.Ignore())
                .ForMember(d => d.LastModifiedBy, opt => opt.Ignore());
        }
    }
}