using AutoMapper;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;

namespace NETForum.Mappings;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<CreatePostDto, Post>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.ForumId, opt => opt.MapFrom(src => src.ForumId))
            .ForMember(dest => dest.IsPinned, opt => opt.MapFrom(src => src.IsPinned))
            .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => src.IsLocked))
            .ForMember(dest => dest.Published, opt => opt.MapFrom(src => src.Published))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        
        CreateMap<EditPostDto, Post>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.ForumId, opt => opt.MapFrom(src => src.ForumId))
            .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
            .ForMember(dest => dest.IsPinned, opt => opt.MapFrom(src => src.IsPinned))
            .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => src.IsLocked))
            .ForMember(dest => dest.Published, opt => opt.MapFrom(src => src.Published))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        
        CreateMap<Post, EditPostDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.ForumId, opt => opt.MapFrom(src => src.ForumId))
            .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
            .ForMember(dest => dest.IsPinned, opt => opt.MapFrom(src => src.IsPinned))
            .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => src.IsLocked))
            .ForMember(dest => dest.Published, opt => opt.MapFrom(src => src.Published));
        
        CreateMap<CreatePostReplyDto, Reply>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.CreatedAt,  opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}