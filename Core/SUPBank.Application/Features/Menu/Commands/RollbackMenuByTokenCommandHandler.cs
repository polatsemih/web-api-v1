using MediatR;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Application.Validations.Menu;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Results;
using SUPBank.Domain.Results.Data;

namespace SUPBank.Application.Features.Menu.Commands
{
    public class RollbackMenusByTokenCommandRequest : IRequest<IResult>
    {
        public required Guid RollbackToken { get; set; }
    }

    public class RollbackMenusByTokenCommandHandler : IRequestHandler<RollbackMenusByTokenCommandRequest, IResult>
    {
        private readonly RollbackMenuByTokenValidator _validator;
        private readonly IMenuQueryRepository _menuQueryRepository;
        private readonly IMenuCommandRepository _menuCommandRepository;

        public RollbackMenusByTokenCommandHandler(RollbackMenuByTokenValidator validator, IMenuQueryRepository menuQueryRepository, IMenuCommandRepository menuCommandRepository)
        {
            _validator = validator;
            _menuQueryRepository = menuQueryRepository;
            _menuCommandRepository = menuCommandRepository;
        }

        public async Task<IResult> Handle(RollbackMenusByTokenCommandRequest request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                string errorMessages = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                return new ErrorResult(errorMessages);
            }

            bool isRollbackTokenExistsInMenuH = await _menuQueryRepository.IsRollbackTokenExistsInMenuHAsync(request.RollbackToken, cancellationToken);
            if (!isRollbackTokenExistsInMenuH)
            {
                return new ErrorResult(ResultMessages.MenuRollbackTokenNotExistInHistory);
            }

            int result = await _menuCommandRepository.RollbackMenusByTokenAsync(request.RollbackToken, cancellationToken);
            if (result > 0)
            {
                return new SuccessDataResult<int>(ResultMessages.MenuRollbackSuccess, result);
            }
            else if (result == -1)
            {
                return new SuccessResult(ResultMessages.MenuRollbackNoChanges);
            }
            return new ErrorResult(ResultMessages.MenuRollbackError);
        }
    }
}
