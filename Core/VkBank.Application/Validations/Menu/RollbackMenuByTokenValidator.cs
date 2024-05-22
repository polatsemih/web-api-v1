using FluentValidation;
using VkBank.Application.Features.Menu.Commands;
using VkBank.Application.Validations.Common;

namespace VkBank.Application.Validations.Menu
{
    public class RollbackMenuByTokenValidator : AbstractValidator<RollbackMenusByTokenCommandRequest>
    {
        public RollbackMenuByTokenValidator()
        {
            RuleFor(r => r.RollbackToken).ValidateRollbackToken();
        }
    }
}
