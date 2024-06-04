using FluentValidation;
using System.Linq.Expressions;
using SUPBank.Domain.Contstants;

namespace SUPBank.Application.Validations.Menu
{
    public static class MenuRules
    {
        public static IRuleBuilderOptions<T, long> ValidateMenuParentId<T>(this IRuleBuilder<T, long> ruleBuilder)
        {
            return ruleBuilder
                .NotNull().WithMessage(ValidationMessages.MenuParentIdNull)
                .GreaterThanOrEqualTo(0).WithMessage(ValidationMessages.MenuParentIdPositiveOrZero);
        }

        public static IRuleBuilderOptions<T, string> ValidateMenuName<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage(ValidationMessages.MenuNameEmpty)
                .MinimumLength(LengthLimits.MenuNameMinLength).WithMessage(string.Format(ValidationMessages.MenuNameMinLength, LengthLimits.MenuNameMinLength))
                .MaximumLength(LengthLimits.MenuNameMaxLength).WithMessage(string.Format(ValidationMessages.MenuNameMaxLength, LengthLimits.MenuNameMaxLength));
        }

        public static IRuleBuilderOptions<T, int?> ValidateMenuScreenCode<T>(this IRuleBuilder<T, int?> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThan(LengthLimits.MenuScreenCodeMinRange).When(screenCode => screenCode != null).WithMessage(string.Format(ValidationMessages.MenuScreenCodeMinRange, LengthLimits.MenuScreenCodeMinRange));
        }

        public static IRuleBuilderOptions<T, int> ValidateMenuScreenCodeRequired<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage(ValidationMessages.MenuScreenCodeEmpty)
                .GreaterThan(LengthLimits.MenuScreenCodeMinRange).WithMessage(string.Format(ValidationMessages.MenuScreenCodeMinRange, LengthLimits.MenuScreenCodeMinRange));
        }

        public static IRuleBuilderOptions<T, string> ValidateMenuWebURL<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage(ValidationMessages.MenuWebURLEmpty)
                .MinimumLength(LengthLimits.MenuWebURLMinLength).WithMessage(string.Format(ValidationMessages.MenuWebURLMinLength, LengthLimits.MenuWebURLMinLength))
                .MaximumLength(LengthLimits.MenuWebURLMaxLength).WithMessage(string.Format(ValidationMessages.MenuWebURLMaxLength, LengthLimits.MenuWebURLMaxLength));
        }

        public static IRuleBuilderOptions<T, byte> ValidateMenuType<T>(this IRuleBuilder<T, byte> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage(ValidationMessages.MenuTypeEmpty);
        }

        public static IRuleBuilderOptions<T, int> ValidateMenuPriority<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder
                .NotNull().WithMessage(ValidationMessages.MenuPriorityNull)
                .GreaterThanOrEqualTo(0).WithMessage(ValidationMessages.MenuPriorityPositiveOrZero);
        }

        public static IRuleBuilderOptions<T, bool> ValidateMenuIsSearch<T>(this IRuleBuilder<T, bool> ruleBuilder)
        {
            return ruleBuilder
                .NotNull().WithMessage(ValidationMessages.MenuIsSearchNull);
        }

        public static IRuleBuilderOptions<T, string> ValidateMenuKeyword<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage(ValidationMessages.MenuKeywordEmpty)
                .MinimumLength(LengthLimits.MenuKeywordMinLength).WithMessage(string.Format(ValidationMessages.MenuKeywordMinLength, LengthLimits.MenuKeywordMinLength))
                .MaximumLength(LengthLimits.MenuKeywordMaxLength).WithMessage(string.Format(ValidationMessages.MenuKeywordMaxLength, LengthLimits.MenuKeywordMaxLength));
        }

        public static IRuleBuilderOptions<T, int> ValidateMenuAuthority<T>(this IRuleBuilder<T, int> ruleBuilder)
        {
            return ruleBuilder
                .NotNull().WithMessage(ValidationMessages.MenuAuthorityNull)
                .GreaterThanOrEqualTo(0).WithMessage(ValidationMessages.MenuAuthorityPositiveOrZero);
        }

        public static IRuleBuilderOptions<T, string?> ValidateMenuIcon<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .MinimumLength(LengthLimits.MenuIconMinLength).When(icon => icon != null).WithMessage(string.Format(ValidationMessages.MenuIconMinLength, LengthLimits.MenuIconMinLength))
                .MaximumLength(LengthLimits.MenuIconMaxLength).When(icon => icon != null).WithMessage(string.Format(ValidationMessages.MenuIconMaxLength, LengthLimits.MenuIconMaxLength));
        }

        public static IRuleBuilderOptions<T, bool> ValidateMenuIsGroup<T>(this IRuleBuilder<T, bool> ruleBuilder)
        {
            return ruleBuilder
                .NotNull().WithMessage(ValidationMessages.MenuIsGroupNull);
        }

        public static IRuleBuilderOptions<T, bool> ValidateMenuIsNew<T>(this IRuleBuilder<T, bool> ruleBuilder)
        {
            return ruleBuilder
                .NotNull().WithMessage(ValidationMessages.MenuIsNewNull);
        }

        public static IRuleBuilderOptions<T, DateTime?> ValidateMenuNewStartDate<T>(this IRuleBuilder<T, DateTime?> ruleBuilder)
        {
            return ruleBuilder
                .Must(date => date == null || date is DateTime).WithMessage(ValidationMessages.MenuNewStartDateInvalid);
        }

        public static IRuleBuilderOptions<T, DateTime?> ValidateMenuNewEndDate<T>(this IRuleBuilder<T, DateTime?> ruleBuilder, Expression<Func<T, DateTime?>> startDateSelector)
        {
            return ruleBuilder
                .Must(date => date == null || date is DateTime).WithMessage(ValidationMessages.MenuNewEndDateInvalid)
                .Must((rootObject, endDate) => endDate == null || endDate > startDateSelector.Compile()(rootObject)).When(endDate => endDate != null).WithMessage(ValidationMessages.MenuNewEndDateMustLater);
        }
    }
}
