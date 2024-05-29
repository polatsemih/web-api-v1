using MediatR;
using System.ComponentModel.DataAnnotations;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Application.Validations.Menu;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Entities;
using SUPBank.Domain.Results;
using SUPBank.Domain.Results.Data;
using Microsoft.IdentityModel.Tokens;

namespace SUPBank.Application.Features.Menu.Queries
{
    public class SearchMenuQueryRequest() : IRequest<IDataResult<List<EntityMenu>>>
    {
        [MinLength(LengthLimits.MenuKeywordMinLength)]
        [MaxLength(LengthLimits.MenuKeywordMaxLength)]
        public required string Keyword { get; set; }
    }

    public class SearchMenuQueryHandler : IRequestHandler<SearchMenuQueryRequest, IDataResult<List<EntityMenu>>>
    {
        private readonly SearchMenuValidator _validator;
        private readonly IMenuQueryRepository _menuQueryRepository;

        public SearchMenuQueryHandler(SearchMenuValidator validator, IMenuQueryRepository menuQueryRepository)
        {
            _validator = validator;
            _menuQueryRepository = menuQueryRepository;
        }

        public async Task<IDataResult<List<EntityMenu>>> Handle(SearchMenuQueryRequest request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return new ErrorDataResult<List<EntityMenu>>(string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage)), new List<EntityMenu>());
            }

            var result = await _menuQueryRepository.SearchMenusAsync(request.Keyword, cancellationToken);
            if (result.IsNullOrEmpty())
            {
                return new ErrorDataResult<List<EntityMenu>>(ResultMessages.MenuNoDatas, new List<EntityMenu>());
            }
            return new SuccessDataResult<List<EntityMenu>>(result.ToList());
        }
    }
}
