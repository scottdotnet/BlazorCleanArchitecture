using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Shared.Common.Attributes;
using BlazorCleanArchitecture.Shared.Common.Exceptions;
using Mediator;
using System.Reflection;

namespace BlazorCleanArchitecture.Application.Common.Behaviours
{
    public sealed class AuthorizationBehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
        where TMessage : IBaseRequest
    {
        private readonly ICurrentUserService _currentUserService;

        public AuthorizationBehaviour(ICurrentUserService currentUserService)
            => _currentUserService = currentUserService;

        public async ValueTask<TResponse> Handle(TMessage message, CancellationToken cancellationToken, MessageHandlerDelegate<TMessage, TResponse> next)
        {
            var permissionAttributes = message.GetType().GetCustomAttributes<PermissionAttribute>().ToList();

            if (permissionAttributes.Count > 0)
            {
                var user = await _currentUserService.User();

                if (user is null)
                    throw new UnauthorizedAccessException();

                var roles = permissionAttributes.Where(x => string.IsNullOrWhiteSpace(x.Role) is false).ToList();

                if (roles.Count > 0)
                {
                    var authorized = false;

                    foreach (var role in roles.Select(r => r.Role))
                    {
                        /*if (user.Role.Name == role)
                        {
                            authorized = true;
                            break;
                        }*/
                    }

                    if (authorized is false)
                        throw new ForbiddenAccessException();
                }

                var permissions = permissionAttributes.Where(x => string.IsNullOrWhiteSpace(x.Permssion) is false).ToList();

                if (permissions.Count > 0)
                {
                    var authorized = false;

                    foreach (var permission in permissions.Select(p => p.Permssion))
                    {
                        /*if (user.Role.Permissions.Select(p => p.Name).Contains(permission))
                        {
                            authorized = true;
                            break;
                        }*/
                    }

                    if (authorized is false)
                        throw new ForbiddenAccessException();
                }
            }

            return await next(message, cancellationToken);
        }
    }
}
