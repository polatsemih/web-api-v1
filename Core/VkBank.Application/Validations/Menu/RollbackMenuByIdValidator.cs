using FluentValidation;
using VkBank.Application.Features.Menu.Commands;
using VkBank.Application.Validations.Common;

namespace VkBank.Application.Validations.Menu
{
    public class RollbackMenuByIdValidator : AbstractValidator<RollbackMenuByIdCommandRequest>
    {
        public RollbackMenuByIdValidator()
        {
            RuleFor(r => r.Id).ValidateId();
        }
    }
}
