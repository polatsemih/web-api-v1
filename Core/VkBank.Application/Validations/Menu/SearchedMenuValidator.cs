using FluentValidation;
using VkBank.Application.Features.Menu.Queries;

namespace VkBank.Application.Validations.Menu
{
    public class SearchMenuValidator : AbstractValidator<SearchMenuQueryRequest>
    {
        public SearchMenuValidator()
        {
            RuleFor(r => r.Keyword).ValidateMenuKeyword();
        }
    }
}
