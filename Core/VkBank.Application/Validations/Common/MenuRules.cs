using FluentValidation;
using System.Linq.Expressions;
using VkBank.Domain.Contstants;
using VkBank.Domain.Entities;

namespace VkBank.Application.Validations.Common
{
    public static class MenuRules
    {
        public static IRuleBuilderOptions<T, long> ValidateMenuParentId<T>(this IRuleBuilder<T, long> ruleBuilder) where T : Menu
        {
            return ruleBuilder
                .NotNull().WithMessage(ValidationMessages.MenuParentIdNull)
                .GreaterThanOrEqualTo(0).WithMessage(ValidationMessages.MenuParentIdPositiveOrZero);
        }

        public static IRuleBuilderOptions<T, string> ValidateMenuName<T>(this IRuleBuilder<T, string> ruleBuilder) where T : Menu
        {
            return ruleBuilder
                .NotEmpty().WithMessage(ValidationMessages.MenuNameEmpty)
                .MinimumLength(LengthLimits.MenuNameMinLength).WithMessage(ValidationMessages.MenuNameMinLength)
                .MaximumLength(LengthLimits.MenuNameMaxLength).WithMessage(ValidationMessages.MenuNameMaxLength);
        }

        public static IRuleBuilderOptions<T, int> ValidateMenuScreenCode<T>(this IRuleBuilder<T, int> ruleBuilder) where T : Menu
        {
            return ruleBuilder
                .NotEmpty().WithMessage(ValidationMessages.MenuScreenCodeEmpty)
                .GreaterThan(10000).WithMessage(ValidationMessages.MenuScreenCodePositive);
        }

        public static IRuleBuilderOptions<T, byte> ValidateMenuType<T>(this IRuleBuilder<T, byte> ruleBuilder) where T : Menu
        {
            return ruleBuilder
                .NotEmpty().WithMessage(ValidationMessages.MenuTypeEmpty);
        }

        public static IRuleBuilderOptions<T, int> ValidateMenuPriority<T>(this IRuleBuilder<T, int> ruleBuilder) where T : Menu
        {
            return ruleBuilder
                .NotNull().WithMessage(ValidationMessages.MenuPriorityNull)
                .GreaterThanOrEqualTo(0).WithMessage(ValidationMessages.MenuPriorityPositiveOrZero);
        }

        public static IRuleBuilderOptions<T, string> ValidateMenuKeyword<T>(this IRuleBuilder<T, string> ruleBuilder) where T : Menu
        {
            return ruleBuilder
                .NotEmpty().WithMessage(ValidationMessages.MenuKeywordEmpty)
                .MinimumLength(LengthLimits.MenuKeywordMinLength).WithMessage(ValidationMessages.MenuKeywordMinLength)
                .MaximumLength(LengthLimits.MenuKeywordMaxLength).WithMessage(ValidationMessages.MenuKeywordMaxLength);
        }

        public static IRuleBuilderOptions<T, string?> ValidateMenuIcon<T>(this IRuleBuilder<T, string?> ruleBuilder) where T : Menu
        {
            return ruleBuilder
                .MinimumLength(LengthLimits.MenuIconMinLength).When(icon => icon != null).WithMessage(ValidationMessages.MenuIconMinLength)
                .MaximumLength(LengthLimits.MenuIconMaxLength).When(icon => icon != null).WithMessage(ValidationMessages.MenuIconMaxLength);
        }

        public static IRuleBuilderOptions<T, bool> ValidateMenuIsGroup<T>(this IRuleBuilder<T, bool> ruleBuilder) where T : Menu
        {
            return ruleBuilder
                .NotNull().WithMessage(ValidationMessages.MenuIsGroupNull);
        }

        public static IRuleBuilderOptions<T, bool> ValidateMenuIsNew<T>(this IRuleBuilder<T, bool> ruleBuilder) where T : Menu
        {
            return ruleBuilder
                .NotNull().WithMessage(ValidationMessages.MenuIsNewEmpty);
        }

        public static IRuleBuilderOptions<T, DateTime?> ValidateMenuNewStartDate<T>(this IRuleBuilder<T, DateTime?> ruleBuilder) where T : Menu
        {
            return ruleBuilder
                .Must(date => date == null || date is DateTime).WithMessage(ValidationMessages.MenuNewStartDateInvalid);
        }

        public static IRuleBuilderOptions<T, DateTime?> ValidateMenuNewEndDate<T>(this IRuleBuilder<T, DateTime?> ruleBuilder, Expression<Func<T, DateTime?>> startDateSelector) where T : Menu
        {
            return ruleBuilder
                .Must(date => date == null || date is DateTime).WithMessage(ValidationMessages.MenuNewEndDateInvalid)
                .Must((rootObject, endDate) => endDate == null || endDate > startDateSelector.Compile()(rootObject)).When(endDate => endDate != null).WithMessage(ValidationMessages.MenuNewEndDateMustLater);
        }
    }
}
