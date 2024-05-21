using FluentValidation;
using VkBank.Application.Features.Commands.UpdateEvent;
using VkBank.Application.Validations.Common;

namespace VkBank.Application.Validations.Update
{
    public class RollbackMenuByDateValidator : AbstractValidator<RollbackMenuByDateCommandRequest>
    {
        public RollbackMenuByDateValidator()
        {
            RuleFor(r => r.ActionType).ValidateRollbackActionType();
        }
    }
}
