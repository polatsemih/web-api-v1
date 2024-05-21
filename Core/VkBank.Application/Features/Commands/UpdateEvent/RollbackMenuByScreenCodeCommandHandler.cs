using MediatR;
using System.ComponentModel.DataAnnotations;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Application.Validations.Update;
using VkBank.Domain.Contstants;
using VkBank.Domain.Results;

namespace VkBank.Application.Features.Commands.UpdateEvent
{
    public class RollbackMenuByScreenCodeCommandRequest : IRequest<IResult>
    {
        [Range(10001, int.MaxValue)]
        public required int ScreenCodeInput { get; set; }

        [Range(1, byte.MaxValue)]
        public required byte ActionType { get; set; }
    }

    public class RollbackMenuByScreenCodeCommandHandler : IRequestHandler<RollbackMenuByScreenCodeCommandRequest, IResult>
    {
        private readonly RollbackMenuByScreenCodeValidator _validator;
        private readonly IMenuRepository _menuRepository;

        public RollbackMenuByScreenCodeCommandHandler(RollbackMenuByScreenCodeValidator validator, IMenuRepository menuRepository)
        {
            _validator = validator;
            _menuRepository = menuRepository;
        }

        public async Task<IResult> Handle(RollbackMenuByScreenCodeCommandRequest request, CancellationToken cancellationToken)
        {
            bool isScreenCodeExists = await _menuRepository.IsMenuScreenCodeExistsAsync(request.ScreenCodeInput, cancellationToken);
            if (!isScreenCodeExists)
            {
                return new ErrorResult(ResultMessages.MenuScreenCodeNotExist);
            }

            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                string errorMessages = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                return new ErrorResult(errorMessages);
            }

            bool rollbackSuccess = await _menuRepository.RollbackMenuByScreenCodeAsync(request.ScreenCodeInput, request.ActionType, cancellationToken);
            return rollbackSuccess ? new SuccessResult(ResultMessages.MenuRollbacked) : new ErrorResult(ResultMessages.MenuRollbackFailed);
        }
    }
}
