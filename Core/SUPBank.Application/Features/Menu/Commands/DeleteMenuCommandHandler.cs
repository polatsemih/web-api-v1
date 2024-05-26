using MediatR;
using System.ComponentModel.DataAnnotations;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Application.Validations.Menu;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Results;

namespace SUPBank.Application.Features.Menu.Commands
{
    public class DeleteMenuCommandRequest : IRequest<IResult>
    {
        [Range(1, long.MaxValue)]
        public required long Id { get; set; }
    }

    public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommandRequest, IResult>
    {
        private readonly DeleteMenuValidator _validator;
        private readonly IMenuQueryRepository _menuQueryRepository;
        private readonly IMenuCommandRepository _menuCommandRepository;

        public DeleteMenuCommandHandler(DeleteMenuValidator validator, IMenuQueryRepository menuQueryRepository, IMenuCommandRepository menuCommandRepository)
        {
            _validator = validator;
            _menuQueryRepository = menuQueryRepository;
            _menuCommandRepository = menuCommandRepository;
        }

        public async Task<IResult> Handle(DeleteMenuCommandRequest request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                string errorMessages = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                return new ErrorResult(errorMessages);
            }

            bool isIdExistsInMenu = await _menuQueryRepository.IsIdExistsInMenuAsync(request.Id, cancellationToken);
            if (!isIdExistsInMenu)
            {
                return new ErrorResult(ResultMessages.MenuIdNotExist);
            }

            bool result = await _menuCommandRepository.DeleteMenuAsync(request.Id, cancellationToken);
            return result ? new SuccessResult(ResultMessages.MenuDeleteSuccess) : new ErrorResult(ResultMessages.MenuDeleteError);
        }
    }
}
