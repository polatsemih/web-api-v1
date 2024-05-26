using FluentValidation;
using SUPBank.Application.Features.Menu.Commands;
using SUPBank.Application.Validations.Common;

namespace SUPBank.Application.Validations.Menu
{
    public class DeleteMenuValidator : AbstractValidator<DeleteMenuCommandRequest>
    {
        public DeleteMenuValidator()
        {
            RuleFor(r => r.Id).ValidateId();
        }
    }
}
