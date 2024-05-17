using FluentValidation;
using VkBank.Application.Validations.Common;
using VkBank.Domain.Entities;

namespace VkBank.Application.Validations.Delete
{
    public class DeleteMenuValidator : AbstractValidator<Menu>
    {
        public DeleteMenuValidator()
        {
            RuleFor(m => m.Id).ValidateId();
        }
    }
}
