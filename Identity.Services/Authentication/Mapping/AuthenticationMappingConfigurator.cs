using AutoMapper;
using Identity.Domain.Authentication;
using Identity.Domain.ViewModel;
using Microsoft.AspNetCore.Identity;

namespace Identity.Services.Authentication.Mapping
{

    public class AuthenticationMappingConfigurator : Profile
    {
        public AuthenticationMappingConfigurator(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<User, LoginViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => GetDisplayName(src)))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ReverseMap();


            cfg.CreateMap<User, UserViewModel>()
                .ReverseMap()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.FullName));

            cfg.CreateMap<IdentityRole, RoleViewModel>()
                .ReverseMap();
        }

        private static string GetDisplayName(User user)
        {
            var result = user.DisplayName;
            if (!string.IsNullOrWhiteSpace(user?.FullName))
            {
                var fullNameParts = user.FullName.Split(' ');
                if (fullNameParts?.Length > 0)
                {
                    result = $"{fullNameParts.FirstOrDefault()?.ToUpper().FirstOrDefault()}";
                    if (fullNameParts.Length > 1)
                    {
                        result += $"{fullNameParts.LastOrDefault()?.ToUpper().FirstOrDefault()}";
                    }
                }
            }
            return result;
        }
    }
}
