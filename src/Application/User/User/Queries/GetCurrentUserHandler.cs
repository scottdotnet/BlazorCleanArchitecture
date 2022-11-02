using AutoMapper;
using BlazorCleanArchitecture.Shared.User.User;
using BlazorCleanArchitecture.Shared.User.User.Queries;
using Mediator;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorCleanArchitecture.Application.User.User.Queries
{
    public sealed class GetCurrentUserHandler : IRequestHandler<GetCurrentUser, UserDto>
    {
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public GetCurrentUserHandler(IMapper mapper, IMemoryCache cache)
            => (_mapper, _cache) = (mapper, cache);

        public async ValueTask<UserDto> Handle(GetCurrentUser request, CancellationToken cancellationToken)
            => _mapper.Map<UserDto>(await Task.FromResult(_cache.Get<Domain.User.User>(nameof(GetCurrentUser))));
    }
}
