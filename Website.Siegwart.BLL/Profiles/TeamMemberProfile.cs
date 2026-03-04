using AutoMapper;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Website.Siegwart.DAL.Models;
using Website.Siegwart.BLL.Dtos.Admin.TeamMember;
using Website.Siegwart.BLL.Dtos.User;
using Website.Siegwart.DAL.Enums;

namespace Website.Siegwart.BLL.Profiles;

public class TeamMemberProfile : Profile
{
    public TeamMemberProfile()
    {
        // ========== Admin Mappings ==========

        // TeamMember → AdminTeamMemberDto (for admin list)
        CreateMap<TeamMember, AdminTeamMemberDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => GetCategoryDisplayName(src.Category)));

        // CreateTeamMemberDto → TeamMember
        CreateMap<CreateTeamMemberDto, TeamMember>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifiedOn, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

        // UpdateTeamMemberDto → TeamMember
        CreateMap<UpdateTeamMemberDto, TeamMember>()
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifiedOn, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore());

        // TeamMember → UpdateTeamMemberDto (for edit form)
        CreateMap<TeamMember, UpdateTeamMemberDto>();

        // ========== User (Frontend) Mappings ==========

        // TeamMember → UserTeamMemberListDto (for public list)
        CreateMap<TeamMember, UserTeamMemberListDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => GetCategoryDisplayName(src.Category)));

        // TeamMember → TeamMemberViewDto (for public details)
        CreateMap<TeamMember, TeamMemberViewDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => GetCategoryDisplayName(src.Category)));
    }

    /// <summary>
    /// Get display name from TeamCategory enum
    /// </summary>
    private static string GetCategoryDisplayName(TeamCategory category)
    {
        var field = category.GetType().GetField(category.ToString());
        var attribute = field?.GetCustomAttribute<DisplayAttribute>();
        return attribute?.Name ?? category.ToString();
    }
}