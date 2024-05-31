using MediatR;
using System.ComponentModel.DataAnnotations;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Application.Validations.Menu;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Results;

namespace SUPBank.Application.Features.Menu.Commands
{
    public class RollbackMenuByScreenCodeCommandRequest : IRequest<IResult>
    {
        [Range(LengthLimits.MenuScreenCodeMinRange, int.MaxValue)]
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
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ErrorResult(string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage)));
            }

            if (!await _menuQueryRepository.IsScreenCodeExistsInMenuHAsync(request.ScreenCode, cancellationToken))
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
