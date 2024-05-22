﻿using FluentValidation;
using VkBank.Domain.Contstants;

namespace VkBank.Application.Validations.Common
{
    public static class SharedRules
    {
        public static IRuleBuilderOptions<T, long> ValidateId<T>(this IRuleBuilder<T, long> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage(ValidationMessages.IdEmpty);
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
