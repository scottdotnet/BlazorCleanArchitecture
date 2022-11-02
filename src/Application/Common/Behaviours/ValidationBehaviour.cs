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
                var validationResults = await Task.WhenAll(_validators.Select(async v => await v.ValidateAsync(message, cancellationToken)));
                var failures = validationResults.Where(r => r.Errors.Any()).SelectMany(r => r.Errors).ToList();

                if (failures.Any())
                    throw new ValidationException(failures);
            }

            return await next(message, cancellationToken);
        }
    }
}
