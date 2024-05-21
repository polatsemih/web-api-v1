using FluentValidation;
using VkBank.Application.Features.Queries.GetAllEvent;
using VkBank.Application.Validations.Common;

namespace VkBank.Application.Validations.Get
{
    public class GetSearchedMenuValidator : AbstractValidator<GetSearchedMenuQueryRequest>
    {
        public GetSearchedMenuValidator()
        {
            RuleFor(r => r.Keyword).ValidateMenuKeyword();
        }
    }
}
