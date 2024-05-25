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
        private readonly IMenuQueryRepository _menuQueryRepository;
        private readonly IMenuCommandRepository _menuCommandRepository;

        public RollbackMenuByScreenCodeCommandHandler(RollbackMenuByScreenCodeValidator validator, IMenuQueryRepository menuQueryRepository, IMenuCommandRepository menuCommandRepository)
        {
            _validator = validator;
            _menuQueryRepository = menuQueryRepository;
            _menuCommandRepository = menuCommandRepository;
        }

        public async Task<IResult> Handle(RollbackMenuByScreenCodeCommandRequest request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                string errorMessages = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                return new ErrorResult(errorMessages);
            }

            bool isScreenCodeExistsInMenuH = await _menuQueryRepository.IsScreenCodeExistsInMenuHAsync(request.ScreenCode, cancellationToken);
            if (!isScreenCodeExistsInMenuH)
            {
                return new ErrorResult(ResultMessages.MenuScreenCodeNotExistInHistory);
            }

            int result = await _menuCommandRepository.RollbackMenuByScreenCodeAsync(request.ScreenCode, cancellationToken);
            if (result == 1)
            {
                return new SuccessResult(ResultMessages.MenuRollbackSuccess);
            } else if (result == -1)
            {
                return new SuccessResult(ResultMessages.MenuRollbackNoChanges);
            }
            return new ErrorResult(ResultMessages.MenuRollbackError);
        }
    }
}
