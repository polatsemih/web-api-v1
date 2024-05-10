using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkBank.Domain.Contstants;
using VkBank.Domain.Entities;

namespace VkBank.Application.Validations.Common
{
    public static class MenuRules
    {
        public static IRuleBuilderOptions<T, string> CheckMenuName<T>(this IRuleBuilder<T, string> ruleBuilder) where T : Menu
        {
            return ruleBuilder
                .NotEmpty().WithMessage(ValidationMessages.MenuNameEmptyError)
                .MinimumLength(LengthLimits.MenuNameMinLength).WithMessage(ValidationMessages.MenuNameMinLengthError)
                .MaximumLength(LengthLimits.MenuNameMaxLength).WithMessage(ValidationMessages.MenuNameMaxLengthError);
        }
    }
}
