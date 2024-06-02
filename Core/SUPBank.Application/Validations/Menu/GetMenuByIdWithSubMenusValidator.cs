using FluentValidation;
using SUPBank.Application.Features.Menu.Queries.Requests;
using SUPBank.Application.Validations.Common;

namespace SUPBank.Application.Validations.Menu
{
    public class GetMenuByIdWithSubMenusValidator : AbstractValidator<GetMenuByIdWithSubMenusQueryRequest>
    {
        public GetMenuByIdWithSubMenusValidator()
        {
            RuleFor(r => r.Id).ValidateId();
        }
    }
}
