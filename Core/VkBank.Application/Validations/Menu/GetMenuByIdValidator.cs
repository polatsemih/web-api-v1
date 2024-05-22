using FluentValidation;
using VkBank.Application.Features.Menu.Queries;
using VkBank.Application.Validations.Common;

namespace VkBank.Application.Validations.Menu
{
    public class GetMenuByIdValidator : AbstractValidator<GetMenuByIdQueryRequest>
    {
        public GetMenuByIdValidator()
        {
            RuleFor(r => r.Id).ValidateId();
        }
    }
}
