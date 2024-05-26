using FluentValidation;
using SUPBank.Application.Features.Menu.Queries;

namespace SUPBank.Application.Validations.Menu
{
    public class SearchMenuValidator : AbstractValidator<SearchMenuQueryRequest>
    {
        public SearchMenuValidator()
        {
            RuleFor(r => r.Keyword).ValidateMenuKeyword();
        }
    }
}
