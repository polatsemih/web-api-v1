using MediatR;
using System.ComponentModel.DataAnnotations;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Application.Validations.Menu;
using VkBank.Domain.Contstants;
using VkBank.Domain.Entities;
using VkBank.Domain.Results;
using VkBank.Domain.Results.Data;

namespace VkBank.Application.Features.Menu.Queries
{
    public class SearchMenuQueryRequest() : IRequest<IResult>
    {
        [MinLength(LengthLimits.MenuKeywordMinLength)]
        [MaxLength(LengthLimits.MenuKeywordMaxLength)]
        public required string Keyword { get; set; }
    }

    public class SearchMenuQueryHandler : IRequestHandler<SearchMenuQueryRequest, IResult>
    {
        private readonly SearchMenuValidator _validator;
        private readonly IMenuQueryRepository _menuQueryRepository;

        public SearchMenuQueryHandler(SearchMenuValidator validator, IMenuQueryRepository menuQueryRepository)
        {
            _validator = validator;
            _menuQueryRepository = menuQueryRepository;
        }

        public async Task<IResult> Handle(SearchMenuQueryRequest request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                string errorMessages = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                return new ErrorResult(errorMessages);
            }

            IEnumerable<EntityMenu> result = await _menuQueryRepository.SearchMenusAsync(request.Keyword, cancellationToken);
            if (!result.Any())
            {
                return new ErrorResult(ResultMessages.MenuNoDatas);
            }
            return new SuccessDataResult<List<EntityMenu>>(result.ToList());
        }
    }
}
