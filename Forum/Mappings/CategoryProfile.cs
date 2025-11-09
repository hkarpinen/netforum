using NETForum.Models.DTOs;
using NETForum.Models.Entities;

namespace NETForum.Mappings;

using AutoMapper;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<CreateCategoryDto, Category>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.SortOrder))
            .ForMember(dest => dest.Published, opt => opt.MapFrom(src => src.Published))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        
        CreateMap<EditCategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.SortOrder))
            .ForMember(dest => dest.Published, opt => opt.MapFrom(src => src.Published))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        
        CreateMap<Category, EditCategoryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.SortOrder))
            .ForMember(dest => dest.Published, opt => opt.MapFrom(src => src.Published))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
        

    }
}