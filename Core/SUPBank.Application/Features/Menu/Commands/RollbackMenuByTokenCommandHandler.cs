using MediatR;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Application.Validations.Menu;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Results.Data;

namespace SUPBank.Application.Features.Menu.Commands
{
    public class RollbackMenusByTokenCommandRequest : IRequest<IDataResult<int>>
    {
        public required Guid RollbackToken { get; set; }
    }

    public class RollbackMenusByTokenCommandHandler : IRequestHandler<RollbackMenusByTokenCommandRequest, IDataResult<int>>
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

        public async Task<IDataResult<int>> Handle(RollbackMenusByTokenCommandRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ErrorDataResult<int>(string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage)));
            }

            if (!await _menuQueryRepository.IsRollbackTokenExistsInMenuHAsync(request.RollbackToken, cancellationToken))
            {
                return new ErrorDataResult<int>(ResultMessages.MenuRollbackTokenNotExistInHistory);
            }

            int result = await _menuCommandRepository.RollbackMenusByTokenAsync(request.RollbackToken, cancellationToken);
            if (result > 0)
            {
                return new SuccessDataResult<int>(ResultMessages.MenuRollbackSuccess, result);
            }
            else if (result == -1)
            {
                return new SuccessDataResult<int>(ResultMessages.MenuRollbackNoChanges);
            }
            return new ErrorDataResult<int>(ResultMessages.MenuRollbackError);
        }
    }
}
