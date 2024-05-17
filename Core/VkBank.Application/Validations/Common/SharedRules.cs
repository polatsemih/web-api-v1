using FluentValidation;
using VkBank.Domain.Contstants;
using VkBank.Domain.Entities;

namespace VkBank.Application.Validations.Common
{
    public static class SharedRules
    {
        public static IRuleBuilderOptions<T, long> ValidateId<T>(this IRuleBuilder<T, long> ruleBuilder) where T : Menu
        {
            return ruleBuilder
                .NotEmpty().WithMessage(ValidationMessages.IdEmpty);
        }

        public static IRuleBuilderOptions<T, bool> ValidateIsActive<T>(this IRuleBuilder<T, bool> ruleBuilder) where T : Menu
        {
            return ruleBuilder
                .NotNull().WithMessage(ValidationMessages.IsActiveNull);
        }
    }
}
