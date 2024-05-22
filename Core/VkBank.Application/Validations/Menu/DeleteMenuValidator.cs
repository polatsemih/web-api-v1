using FluentValidation;
using VkBank.Application.Features.Menu.Commands;
using VkBank.Application.Validations.Common;

namespace VkBank.Application.Validations.Menu
{
    public class DeleteMenuValidator : AbstractValidator<DeleteMenuCommandRequest>
    {
        public DeleteMenuValidator()
        {
            RuleFor(r => r.Id).ValidateId();
        }
    }
}
