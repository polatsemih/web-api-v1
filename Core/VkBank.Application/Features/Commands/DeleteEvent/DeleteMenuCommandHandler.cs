using AutoMapper;
using MediatR;
using System.ComponentModel.DataAnnotations;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Application.Validations.Delete;
using VkBank.Domain.Contstants;
using VkBank.Domain.Entities;
using VkBank.Domain.Results;

namespace VkBank.Application.Features.Commands.DeleteEvent
{
    public class DeleteMenuCommandRequest : IRequest<IResult>
    {
        [Range(1, long.MaxValue)]
        public required long Id { get; set; }
    }

    public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommandRequest, IResult>
    {
        private readonly IMapper _mapper;
        private readonly DeleteMenuValidator _validator;
        private readonly IMenuRepository _menuRepository;

        public DeleteMenuCommandHandler(IMapper mapper, DeleteMenuValidator validator, IMenuRepository menuRepository)
        {
            _mapper = mapper;
            _validator = validator;
            _menuRepository = menuRepository;
        }

        public async Task<IResult> Handle(DeleteMenuCommandRequest request, CancellationToken cancellationToken)
        {
            bool isIdExists = await _menuRepository.IsMenuIdExistsAsync(request.Id, cancellationToken);
            if (!isIdExists)
            {
                return new ErrorResult(ResultMessages.MenuIdNotExist);
            }

            Menu menu = _mapper.Map<Menu>(request);

            var validationResult = _validator.Validate(menu);
            if (!validationResult.IsValid)
            {
                string errorMessages = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                return new ErrorResult(errorMessages);
            }

            bool deleteSuccess = await _menuRepository.DeleteMenuAsync(menu.Id, cancellationToken);
            return deleteSuccess ? new SuccessResult(ResultMessages.MenuDeleted) : new ErrorResult(ResultMessages.MenuDeleteFailed);
        }
    }
}
