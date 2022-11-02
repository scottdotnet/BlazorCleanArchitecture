using FluentValidation;
using Mediator;
using ValidationException = BlazorCleanArchitecture.Shared.Common.Exceptions.ValidationException;

namespace BlazorCleanArchitecture.Application.Common.Behaviours
{
    public sealed class ValidationBehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
        where TMessage : IMessage
    {
        private readonly IEnumerable<IValidator<TMessage>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TMessage>> validators)
        {
            _validators = validators;
        }

        public async ValueTask<TResponse> Handle(TMessage message, CancellationToken cancellationToken, MessageHandlerDelegate<TMessage, TResponse> next)
        {
            if (_validators.Any())
            {
                foreach (var validator in _validators)
                {
                    var result = await validator.ValidateAsync(message, cancellationToken);

                    if (result.Errors.Count > 0)
                        throw new ValidationException(result.Errors);
                }
            }

            return await next(message, cancellationToken);
        }
    }
}
