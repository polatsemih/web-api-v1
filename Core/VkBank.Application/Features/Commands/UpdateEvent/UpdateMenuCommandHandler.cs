using AutoMapper;
using MediatR;
using System.ComponentModel.DataAnnotations;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Application.Validations.Update;
using VkBank.Domain.Contstants;
using VkBank.Domain.Entities;
using VkBank.Domain.Results;

namespace VkBank.Application.Features.Commands.UpdateEvent
{
    public class UpdateMenuCommandRequest : IRequest<IResult>
    {
        [Range(1, long.MaxValue)]
        public required long Id { get; set; }

        [Range(0, long.MaxValue)]
        public required long ParentId { get; set; }

        [MinLength(LengthLimits.MenuNameMinLength)]
        [MaxLength(LengthLimits.MenuNameMaxLength)]
        public required string Name_TR { get; set; }

        [MinLength(LengthLimits.MenuNameMinLength)]
        [MaxLength(LengthLimits.MenuNameMaxLength)]
        public required string Name_EN { get; set; }

        [Range(10001, int.MaxValue)]
        public required int ScreenCode { get; set; }

        [Range(1, byte.MaxValue)]
        public required byte Type { get; set; }

        [Range(1, int.MaxValue)]
        public required int Priority { get; set; }

        [MinLength(LengthLimits.MenuKeywordMinLength)]
        [MaxLength(LengthLimits.MenuKeywordMaxLength)]
        public required string Keyword { get; set; }

        [MinLength(LengthLimits.MenuIconMinLength)]
        [MaxLength(LengthLimits.MenuIconMaxLength)]
        public string? Icon { get; set; }

        public required bool IsGroup { get; set; }

        public required bool IsNew { get; set; }

        public DateTime? NewStartDate { get; set; }

        public DateTime? NewEndDate { get; set; }

        public required bool IsActive { get; set; }
    }

    public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommandRequest, IResult>
    {
        private readonly IMapper _mapper;
        private readonly UpdateMenuValidator _validator;
        private readonly IMenuRepository _menuRepository;

        public UpdateMenuCommandHandler(IMapper mapper, UpdateMenuValidator validator, IMenuRepository menuRepository)
        {
            _mapper = mapper;
            _validator = validator;
            _menuRepository = menuRepository;
        }

        public async Task<IResult> Handle(UpdateMenuCommandRequest request, CancellationToken cancellationToken)
        {
            bool isIdExists = await _menuRepository.IsMenuIdExistsAsync(request.Id, cancellationToken);
            if (!isIdExists)
            {
                return new ErrorResult(ResultMessages.MenuIdNotExist);
            }

            if (request.ParentId != 0)
            {
                bool isParentIdExists = await _menuRepository.IsMenuParentIdExistsAsync(request.ParentId, cancellationToken);
                if (!isParentIdExists)
                {
                    return new ErrorResult(ResultMessages.MenuParentIdNotExist);
                }
            }

            Menu menu = _mapper.Map<Menu>(request);

            var validationResult = _validator.Validate(menu);
            if (!validationResult.IsValid)
            {
                string errorMessages = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                return new ErrorResult(errorMessages);
            }

            bool updateSuccess = await _menuRepository.UpdateMenuAsync(menu, cancellationToken);
            return updateSuccess ? new SuccessResult(ResultMessages.MenuUpdated) : new ErrorResult(ResultMessages.MenuUpdateFailed);
        }
    }
}
