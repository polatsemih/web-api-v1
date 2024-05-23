using MediatR;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Application.Validations.Menu;
using VkBank.Domain.Contstants;
using VkBank.Domain.Entities;
using VkBank.Domain.Results.Data;

namespace VkBank.Application.Features.Menu.Queries
{
    public class GetMenuByIdQueryRequest : IRequest<IDataResult<List<EntityMenu>>>
    {
        public long Id { get; set; }
    }

    public class GetMenuByIdQueryHandler : IRequestHandler<GetMenuByIdQueryRequest, IDataResult<List<EntityMenu>>>
    {
        private readonly GetMenuByIdValidator _validator;
        private readonly IMenuRepository _menuRepository;

        public GetMenuByIdQueryHandler(GetMenuByIdValidator validator, IMenuRepository menuRepository)
        {
            _validator = validator;
            _menuRepository = menuRepository;
        }

        public async Task<IDataResult<List<EntityMenu>>> Handle(GetMenuByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                string errorMessages = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                return new ErrorDataResult<List<EntityMenu>>(errorMessages);
            }

            var result = await _menuRepository.GetMenuByIdAsync(request.Id, cancellationToken);
            if (result == null)
            {
                return new ErrorDataResult<List<EntityMenu>>(ResultMessages.MenuNoData);
            }
            return new SuccessDataResult<List<EntityMenu>>(result.ToList());
        }
    }
}
