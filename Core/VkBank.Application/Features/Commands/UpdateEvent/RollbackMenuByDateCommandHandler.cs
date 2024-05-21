using MediatR;
using System.ComponentModel.DataAnnotations;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Application.Validations.Update;
using VkBank.Domain.Contstants;
using VkBank.Domain.Results;

namespace VkBank.Application.Features.Commands.UpdateEvent
{
    public class RollbackMenuByDateCommandRequest : IRequest<IResult>
    {
        public required DateTime Date { get; set; }

        [Range(1, byte.MaxValue)]
        public required byte ActionType { get; set; }
    }

    public class RollbackMenuByDateCommandHandler : IRequestHandler<RollbackMenuByDateCommandRequest, IResult>
    {
        private readonly RollbackMenuByDateValidator _validator;
        private readonly IMenuRepository _menuRepository;

        public RollbackMenuByDateCommandHandler(RollbackMenuByDateValidator validator, IMenuRepository menuRepository)
        {
            _validator = validator;
            _menuRepository = menuRepository;
        }

        public async Task<IResult> Handle(RollbackMenuByDateCommandRequest request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                string errorMessages = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                return new ErrorResult(errorMessages);
            }

            int? result = await _menuRepository.RollbackMenuByDateAsync(request.Date, request.ActionType, cancellationToken);
            if (result == null)
            {
                return new ErrorResult(ResultMessages.MenuRollbackFailed);
            }
            return new SuccessResult(ResultMessages.MenuRollbacked);
        }
    }
}
