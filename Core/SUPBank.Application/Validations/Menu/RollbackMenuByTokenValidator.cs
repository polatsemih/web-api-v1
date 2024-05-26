using FluentValidation;
using SUPBank.Application.Features.Menu.Commands;
using SUPBank.Application.Validations.Common;

namespace SUPBank.Application.Validations.Menu
{
    public class RollbackMenuByTokenValidator : AbstractValidator<RollbackMenusByTokenCommandRequest>
    {
        public RollbackMenuByTokenValidator()
        {
            RuleFor(r => r.RollbackToken).ValidateRollbackToken();
        }
    }
}
