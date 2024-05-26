using MediatR;
using System.ComponentModel.DataAnnotations;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Application.Validations.Menu;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Results;

namespace SUPBank.Application.Features.Menu.Commands
{
    public class RollbackMenuByIdCommandRequest : IRequest<IResult>
    {
        [Range(1, long.MaxValue)]
        public required long Id { get; set; }
    }

    public class RollbackMenuByIdCommandHandler : IRequestHandler<RollbackMenuByIdCommandRequest, IResult>
    {
        private readonly RollbackMenuByIdValidator _validator;
        private readonly IMenuQueryRepository _menuQueryRepository;
        private readonly IMenuCommandRepository _menuCommandRepository;

        public RollbackMenuByIdCommandHandler(RollbackMenuByIdValidator validator, IMenuQueryRepository menuQueryRepository, IMenuCommandRepository menuCommandRepository)
        {
            _validator = validator;
            _menuQueryRepository = menuQueryRepository;
            _menuCommandRepository = menuCommandRepository;
        }

        public async Task<IResult> Handle(RollbackMenuByIdCommandRequest request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                string errorMessages = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                return new ErrorResult(errorMessages);
            }

            bool IsMenuIdExistsInMenuH = await _menuQueryRepository.IsMenuIdExistsInMenuHAsync(request.Id, cancellationToken);
            if (!IsMenuIdExistsInMenuH)
            {
                return new ErrorResult(ResultMessages.MenuIdNotExistInHistory);
            }

            int result = await _menuCommandRepository.RollbackMenuByIdAsync(request.Id, cancellationToken);
            if (result == 1)
            {
                return new SuccessResult(ResultMessages.MenuRollbackSuccess);
            }
            else if (result == -1)
            {
                return new SuccessResult(ResultMessages.MenuRollbackNoChanges);
            }
            return new ErrorResult(ResultMessages.MenuRollbackError);
        }
    }
}
