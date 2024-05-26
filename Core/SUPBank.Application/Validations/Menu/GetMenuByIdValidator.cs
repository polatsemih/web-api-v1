using FluentValidation;
using SUPBank.Application.Features.Menu.Queries;
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

    public class GetMenuByIdWithSubMenusQueryRequestValidator : AbstractValidator<GetMenuByIdWithSubMenusQueryRequest>
    {
        public GetMenuByIdWithSubMenusQueryRequestValidator()
        {
            RuleFor(r => r.Id).ValidateId();
        }
    }
}
