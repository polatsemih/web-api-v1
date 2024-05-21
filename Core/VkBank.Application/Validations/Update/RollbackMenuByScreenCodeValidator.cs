using FluentValidation;
using VkBank.Application.Features.Commands.UpdateEvent;
using VkBank.Application.Validations.Common;

namespace VkBank.Application.Validations.Update
{
    public class RollbackMenuByScreenCodeValidator : AbstractValidator<RollbackMenuByScreenCodeCommandRequest>
    {
        public RollbackMenuByScreenCodeValidator()
        {
            RuleFor(r => r.ScreenCodeInput).ValidateMenuScreenCode();
            RuleFor(r => r.ActionType).ValidateRollbackActionType();
        }
    }
}
