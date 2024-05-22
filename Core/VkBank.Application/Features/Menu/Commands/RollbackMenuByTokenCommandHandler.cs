using MediatR;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Application.Validations.Menu;
using VkBank.Domain.Contstants;
using VkBank.Domain.Entities;
using VkBank.Domain.Results;
using VkBank.Domain.Results.Data;

namespace VkBank.Application.Features.Menu.Commands
{
    public class RollbackMenusByTokenCommandRequest : IRequest<IResult>
    {
        public required Guid RollbackToken { get; set; }
    }

    public class RollbackMenusByTokenCommandHandler : IRequestHandler<RollbackMenusByTokenCommandRequest, IResult>
    {
        private readonly RollbackMenuByTokenValidator _validator;
        private readonly IMenuRepository _menuRepository;

        public RollbackMenusByTokenCommandHandler(RollbackMenuByTokenValidator validator, IMenuRepository menuRepository)
        {
            _validator = validator;
            _menuRepository = menuRepository;
        }

        public async Task<IResult> Handle(RollbackMenusByTokenCommandRequest request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                string errorMessages = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                return new ErrorResult(errorMessages);
            }

            int? result = await _menuRepository.RollbackMenusByTokenAsync(request.RollbackToken, cancellationToken);
            if (result == null)
            {
                return new ErrorResult(ResultMessages.MenuRollbackError);
            }
            return new SuccessDataResult<int?>(ResultMessages.MenuRollbackSuccess, result);
        }
    }
}
