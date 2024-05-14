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

        public static IRuleBuilderOptions<T, long> ValidateMenuId<T>(this IRuleBuilder<T, long> ruleBuilder) where T : Menu
        {
            return ruleBuilder.NotEmpty().WithMessage(ValidationMessages.MenuIdEmpty);
        }

        public static IRuleBuilderOptions<T, string> ValidateMenuName<T>(this IRuleBuilder<T, string> ruleBuilder) where T : Menu
        {
            return ruleBuilder
                .NotEmpty().WithMessage(ValidationMessages.MenuNameEmpty)
                .MinimumLength(LengthLimits.MenuNameMinLength).WithMessage(ValidationMessages.MenuNameMinLength)
                .MaximumLength(LengthLimits.MenuNameMaxLength).WithMessage(ValidationMessages.MenuNameMaxLength);
        }
    }
}
