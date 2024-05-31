using MediatR;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Application.Validations.Menu;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Entities;
using SUPBank.Domain.Results.Data;
using System.ComponentModel.DataAnnotations;

namespace SUPBank.Application.Features.Menu.Queries
{
    public class GetMenuByIdQueryRequest : IRequest<IDataResult<EntityMenu>>
    {
        [Range(1, long.MaxValue)]
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
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ErrorDataResult<EntityMenu>(string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage)));
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
