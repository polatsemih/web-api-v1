using MediatR;
using System.ComponentModel.DataAnnotations;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Application.Validations.Update;
using VkBank.Domain.Contstants;
using VkBank.Domain.Results;

namespace VkBank.Application.Features.Commands.UpdateEvent
{
    public class RollbackMenuByIdCommandRequest : IRequest<IResult>
    {
        [Range(1, long.MaxValue)]
        public required long Id { get; set; }

        [Range(1, byte.MaxValue)]
        public required byte ActionType { get; set; }
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
            bool isIdExists = await _menuRepository.IsMenuIdExistsAsync(request.Id, cancellationToken);
            if (!isIdExists)
            {
                return new ErrorResult(ResultMessages.MenuIdNotExist);
            }

            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                string errorMessages = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                return new ErrorResult(errorMessages);
            }

            bool rollbackSuccess = await _menuRepository.RollbackMenuByIdAsync(request.Id, request.ActionType, cancellationToken);
            return rollbackSuccess ? new SuccessResult(ResultMessages.MenuRollbacked) : new ErrorResult(ResultMessages.MenuRollbackFailed);
        }
    }
}
