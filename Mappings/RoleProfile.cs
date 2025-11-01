using AutoMapper;
using NETForum.Models;
using NETForum.Pages.Roles;

namespace NETForum.Mappings;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<CreateRoleDto, Role>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.NormalizedName, opt => opt.MapFrom(src => src.Name.ToUpper()));

        CreateMap<EditRoleDto, Role>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.NormalizedName, opt => opt.MapFrom(src => src.Name.ToUpper()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
        
        CreateMap<Role, EditRoleDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
    }
}