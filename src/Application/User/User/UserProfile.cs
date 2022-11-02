using AutoMapper;
using BlazorCleanArchitecture.Shared.User.User;

namespace BlazorCleanArchitecture.Application.User.User
{
    public sealed class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Domain.User.User, UserDto>()
                .ReverseMap()
                .ForMember(d => d.Id, opt => opt.Ignore());
        }
    }
}
