using MediatR;
using System.ComponentModel.DataAnnotations;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Application.Validations.Menu;
using VkBank.Domain.Contstants;
using VkBank.Domain.Results;

namespace VkBank.Application.Features.Menu.Commands
{
    public class DeleteMenuCommandRequest : IRequest<IResult>
    {
        [Range(1, long.MaxValue)]
        public required long Id { get; set; }
    }

    public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommandRequest, IResult>
    {
        private readonly DeleteMenuValidator _validator;
        private readonly IMenuRepository _menuRepository;

        public DeleteMenuCommandHandler(DeleteMenuValidator validator, IMenuRepository menuRepository)
        {
            _validator = validator;
            _menuRepository = menuRepository;
        }

        public async Task<IResult> Handle(DeleteMenuCommandRequest request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                string errorMessages = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                return new ErrorResult(errorMessages);
            }

            bool isIdExistsInMenu = await _menuRepository.IsIdExistsInMenuAsync(request.Id, cancellationToken);
            if (!isIdExistsInMenu)
            {
                return new ErrorResult(ResultMessages.MenuIdNotExist);
            }

            bool result = await _menuRepository.DeleteMenuAsync(request.Id, cancellationToken);
            return result ? new SuccessResult(ResultMessages.MenuDeleteSuccess) : new ErrorResult(ResultMessages.MenuDeleteError);
        }
    }
}
