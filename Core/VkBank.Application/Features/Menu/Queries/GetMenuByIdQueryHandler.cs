using MediatR;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Application.Validations.Menu;
using VkBank.Domain.Contstants;
using VkBank.Domain.Entities;
using VkBank.Domain.Results.Data;

namespace VkBank.Application.Features.Menu.Queries
{
    public class GetMenuByIdQueryRequest : IRequest<IDataResult<EntityMenu>>
    {
        public long Id { get; set; }
    }

    public class GetMenuByIdQueryHandler : IRequestHandler<GetMenuByIdQueryRequest, IDataResult<EntityMenu>>
    {
        private readonly GetMenuByIdValidator _validator;
        private readonly IMenuQueryRepository _menuQueryRepository;

        public GetMenuByIdQueryHandler(GetMenuByIdValidator validator, IMenuQueryRepository menuQueryRepository)
        {
            _validator = validator;
            _menuQueryRepository = menuQueryRepository;
        }

        public async Task<IDataResult<EntityMenu>> Handle(GetMenuByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                string errorMessages = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                return new ErrorDataResult<EntityMenu>(errorMessages);
            }

            var result = await _menuQueryRepository.GetMenuByIdAsync(request.Id, cancellationToken);
            if (result == null)
            {
                return new ErrorDataResult<EntityMenu>(ResultMessages.MenuNoData);
            }
            return new SuccessDataResult<EntityMenu>(result);
        }
    }
}
