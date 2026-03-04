using Website.Siegwart.BLL.Dtos.Admin.VideoMedia;

namespace Website.Siegwart.BLL.Profiles
{
    public class VideoMediaProfile : Profile
    {
        public VideoMediaProfile()
        {
            // Entity -> Admin full DTO (used in admin details/list)
            CreateMap<VideoMedia, VideoMediaDto>();

            // Entity -> PL list item DTO (used for public lists / gallery)
            CreateMap<VideoMedia, VideoMediaListItemDto>();

            CreateMap<VideoMediaCreateDto, VideoMedia>()
                .ForMember(dest => dest.VideoId, opt => opt.Ignore())
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.Ignore())
                .ForMember(dest => dest.SourceUrl, opt => opt.MapFrom(src => src.VideoUrl));

            // Update DTO -> Entity
            CreateMap<VideoMediaUpdateDto, VideoMedia>()
                .ForMember(dest => dest.VideoId, opt => opt.Ignore())
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.Ignore())
                .ForMember(dest => dest.SourceUrl, opt => opt.MapFrom(src => src.VideoUrl));
        }
    }
}