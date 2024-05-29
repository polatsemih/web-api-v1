using MediatR;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Application.Validations.Menu;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Entities;
using SUPBank.Domain.Results;
using SUPBank.Domain.Results.Data;

namespace SUPBank.Application.Features.Menu.Queries
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
                return new ErrorDataResult<EntityMenu>(string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage)), new EntityMenu());
            }

            var result = await _menuQueryRepository.GetMenuByIdAsync(request.Id, cancellationToken);
            if (result == null)
            {
                return new ErrorDataResult<EntityMenu>(ResultMessages.MenuNoData, new EntityMenu());
            }
            return new SuccessDataResult<EntityMenu>(result);
        }
    }
}
