using FluentValidation;
using SUPBank.Application.Features.Menu.Queries.Requests;
using SUPBank.Application.Validations.Common;

namespace SUPBank.Application.Validations.Menu
{
    public class GetMenuByIdValidator : AbstractValidator<GetMenuByIdQueryRequest>
    {
        public GetMenuByIdValidator()
        {
            RuleFor(r => r.Id).ValidateId();
        }
    }

    public class GetMenuByIdWithSubMenusValidator : AbstractValidator<GetMenuByIdWithSubMenusQueryRequest>
    {
        public GetMenuByIdWithSubMenusValidator()
        {
            RuleFor(r => r.Id).ValidateId();
        }
    }
}
