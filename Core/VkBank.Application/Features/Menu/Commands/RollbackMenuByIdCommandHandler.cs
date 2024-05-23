using MediatR;
using System.ComponentModel.DataAnnotations;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Application.Validations.Menu;
using VkBank.Domain.Contstants;
using VkBank.Domain.Results;

namespace VkBank.Application.Features.Menu.Commands
{
    public class RollbackMenuByIdCommandRequest : IRequest<IResult>
    {
        [Range(1, long.MaxValue)]
        public required long Id { get; set; }
    }

    public class RollbackMenuByIdCommandHandler : IRequestHandler<RollbackMenuByIdCommandRequest, IResult>
    {
        private readonly RollbackMenuByIdValidator _validator;
        private readonly IMenuRepository _menuRepository;

        public RollbackMenuByIdCommandHandler(RollbackMenuByIdValidator validator, IMenuRepository menuRepository)
        {
            _validator = validator;
            _menuRepository = menuRepository;
        }

        public async Task<IResult> Handle(RollbackMenuByIdCommandRequest request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                string errorMessages = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                return new ErrorResult(errorMessages);
            }

            bool isMenuIdExistsInHistory = await _menuRepository.IsMenuIdExistsAtHistoryAsync(request.Id, cancellationToken);
            if (!isMenuIdExistsInHistory)
            {
                return new ErrorResult(ResultMessages.MenuIdNotExistInHistory);
            }

            bool rollbackSuccess = await _menuRepository.RollbackMenuByIdAsync(request.Id, cancellationToken);
            return rollbackSuccess ? new SuccessResult(ResultMessages.MenuRollbackSuccess) : new ErrorResult(ResultMessages.MenuRollbackError);
        }
    }
}
