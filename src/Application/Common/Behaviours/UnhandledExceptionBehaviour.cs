using Mediator;

namespace BlazorCleanArchitecture.Application.Common.Behaviours
{
    public sealed class UnhandledExceptionBehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
        where TMessage : IMessage
    {
        public async ValueTask<TResponse> Handle(TMessage message, CancellationToken cancellationToken, MessageHandlerDelegate<TMessage, TResponse> next)
        {
            try
            {
                return await next(message, cancellationToken);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
