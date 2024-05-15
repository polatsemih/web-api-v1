using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkBank.Application.Interfaces.Repositories;
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

        public static IRuleBuilderOptions<T, long?> ValidateMenuParentId<T>(this IRuleBuilder<T, long?> ruleBuilder) where T : Menu
        {
            return ruleBuilder.GreaterThan(0).When(m => m.ParentId.HasValue).WithMessage("ParentId must be greater than 0.");
        }

        public static IRuleBuilderOptions<T, string> ValidateMenuName<T>(this IRuleBuilder<T, string> ruleBuilder) where T : Menu
        {
            return ruleBuilder
                .NotEmpty().WithMessage(ValidationMessages.MenuNameEmpty)
                .MinimumLength(LengthLimits.MenuNameMinLength).WithMessage(ValidationMessages.MenuNameMinLength)
                .MaximumLength(LengthLimits.MenuNameMaxLength).WithMessage(ValidationMessages.MenuNameMaxLength);
        }

        public static IRuleBuilderOptions<T, byte> ValidateMenuType<T>(this IRuleBuilder<T, byte> ruleBuilder) where T : Menu
        {
            return ruleBuilder.NotEmpty().WithMessage(ValidationMessages.MenuTypeEmpty);
        }

        public static IRuleBuilderOptions<T, int?> ValidateMenuPriority<T>(this IRuleBuilder<T, int?> ruleBuilder) where T : Menu
        {
            return ruleBuilder.GreaterThan(0).When(m => m.Priority.HasValue).WithMessage("Priority must be greater than 0.");
        }

        public static IRuleBuilderOptions<T, string> ValidateMenuKeyword<T>(this IRuleBuilder<T, string> ruleBuilder) where T : Menu
        {
            return ruleBuilder
                .NotEmpty().WithMessage(ValidationMessages.MenuKeywordEmpty)
                .MinimumLength(LengthLimits.MenuKeywordMinLength).WithMessage(ValidationMessages.MenuKeywordMinLength)
                .MaximumLength(LengthLimits.MenuKeywordMaxLength).WithMessage(ValidationMessages.MenuKeywordMaxLength);
        }
    }
}
