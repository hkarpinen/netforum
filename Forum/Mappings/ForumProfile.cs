using AutoMapper;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;

namespace NETForum.Mappings;

public class ForumProfile : Profile
{
    public ForumProfile()
    {
        CreateMap<CreateForumDto, Forum>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Published, opt => opt.MapFrom(src => src.Published))
            .ForMember(dest => dest.ParentForumId, opt => opt.MapFrom(src => src.ParentForumId))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        
        CreateMap<EditForumDto, Forum>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Published, opt => opt.MapFrom(src => src.Published))
            .ForMember(dest => dest.ParentForumId, opt => opt.MapFrom(src => src.ParentForumId))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<Forum, EditForumDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Published, opt => opt.MapFrom(src => src.Published))
            .ForMember(dest => dest.ParentForumId, opt => opt.MapFrom(src => src.ParentForumId))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId));
    }
}