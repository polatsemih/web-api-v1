using FluentValidation;
using MediatR;
using System.ComponentModel.DataAnnotations;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Application.Validations.Delete;
using VkBank.Application.Validations.Get;
using VkBank.Domain.Contstants;
using VkBank.Domain.Entities;
using VkBank.Domain.Results;
using VkBank.Domain.Results.Data;

namespace VkBank.Application.Features.Queries.GetAllEvent
{
    public class GetSearchedMenuQueryRequest() : IRequest<IDataResult<List<Menu>>>
    {
        [MinLength(LengthLimits.MenuKeywordMinLength)]
        [MaxLength(LengthLimits.MenuKeywordMaxLength)]
        public required string Keyword { get; set; }
    }

    public class GetSearchedMenuQueryHandler : IRequestHandler<GetSearchedMenuQueryRequest, IDataResult<List<Menu>>>
    {
        private readonly GetSearchedMenuValidator _validator;
        private readonly IMenuRepository _menuRepository;

        public GetSearchedMenuQueryHandler(GetSearchedMenuValidator validator, IMenuRepository menuRepository)
        {
            _validator = validator;
            _menuRepository = menuRepository;
        }

        public async Task<IDataResult<List<Menu>>> Handle(GetSearchedMenuQueryRequest request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                string errorMessages = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                return new ErrorDataResult<List<Menu>>(errorMessages, null);
            }

            IEnumerable<Menu> result = await _menuRepository.GetSearchedMenuAsync(request.Keyword, cancellationToken);
            if (!result.Any())
            {
                return new ErrorDataResult<List<Menu>>(ResultMessages.MenuNoDatas, null);
            }
            return new SuccessDataResult<List<Menu>>(result.ToList());
        }
    }
}
