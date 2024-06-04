using FluentValidation;
using SUPBank.Domain.Contstants;

namespace SUPBank.Application.Validations.Common
{
    public static class SharedRules
    {
        public static IRuleBuilderOptions<T, long> ValidateId<T>(this IRuleBuilder<T, long> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage(ValidationMessages.IdEmpty)
                .GreaterThan(0).WithMessage(ValidationMessages.IdPositive);
        }

        public static IRuleBuilderOptions<T, bool> ValidateIsActive<T>(this IRuleBuilder<T, bool> ruleBuilder)
        {
            return ruleBuilder
                .NotNull().WithMessage(ValidationMessages.IsActiveNull);
        }

        public static IRuleBuilderOptions<T, Guid> ValidateRollbackToken<T>(this IRuleBuilder<T, Guid> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage(ValidationMessages.RollbackTokenEmpty);
        }
    }
}
