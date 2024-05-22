using FluentValidation;
using VkBank.Application.Features.Menu.Commands;

namespace VkBank.Application.Validations.Menu
{
    public class RollbackMenuByScreenCodeValidator : AbstractValidator<RollbackMenuByScreenCodeCommandRequest>
    {
        public RollbackMenuByScreenCodeValidator()
        {
            RuleFor(r => r.ScreenCode).ValidateMenuScreenCode();
        }
    }
}
