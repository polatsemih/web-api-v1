using FluentValidation;
using SUPBank.Application.Features.Menu.Commands;
using SUPBank.Application.Validations.Common;

namespace SUPBank.Application.Validations.Menu
{
    public class RollbackMenuByIdValidator : AbstractValidator<RollbackMenuByIdCommandRequest>
    {
        public RollbackMenuByIdValidator()
        {
            RuleFor(r => r.Id).ValidateId();
        }
    }
}
