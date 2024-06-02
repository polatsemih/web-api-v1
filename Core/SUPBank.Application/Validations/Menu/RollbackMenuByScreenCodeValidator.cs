using FluentValidation;
using SUPBank.Application.Features.Menu.Commands.Requests;

namespace SUPBank.Application.Validations.Menu
{
    public class RollbackMenuByScreenCodeValidator : AbstractValidator<RollbackMenuByScreenCodeCommandRequest>
    {
        public RollbackMenuByScreenCodeValidator()
        {
            RuleFor(r => r.ScreenCode).ValidateMenuScreenCode();
        }
    }
}
