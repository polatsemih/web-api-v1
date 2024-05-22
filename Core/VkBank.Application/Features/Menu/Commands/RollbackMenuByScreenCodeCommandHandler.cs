using MediatR;
using System.ComponentModel.DataAnnotations;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Application.Validations.Menu;
using VkBank.Domain.Contstants;
using VkBank.Domain.Results;

namespace VkBank.Application.Features.Menu.Commands
{
    public class RollbackMenuByScreenCodeCommandRequest : IRequest<IResult>
    {
        [Range(10001, int.MaxValue)]
        public required int ScreenCode { get; set; }
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
            bool isScreenCodeExists = await _menuRepository.IsMenuScreenCodeExistsAsync(request.ScreenCode, cancellationToken);
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

            bool rollbackSuccess = await _menuRepository.RollbackMenuByScreenCodeAsync(request.ScreenCode, cancellationToken);
            return rollbackSuccess ? new SuccessResult(ResultMessages.MenuRollbackSuccess) : new ErrorResult(ResultMessages.MenuRollbackError);
        }
    }
}
