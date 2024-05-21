using FluentValidation;
using VkBank.Application.Features.Commands.UpdateEvent;
using VkBank.Application.Validations.Common;

namespace VkBank.Application.Validations.Update
{
    public class RollbackMenuByIdValidator : AbstractValidator<RollbackMenuByIdCommandRequest>
    {
        public RollbackMenuByIdValidator()
        {
            RuleFor(r => r.Id).ValidateId();
        }
    }
}
