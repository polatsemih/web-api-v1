using AutoMapper;
using MediatR;
using System.ComponentModel.DataAnnotations;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Application.Validations.Menu;
using VkBank.Domain.Contstants;
using VkBank.Domain.Results;
using VkBank.Domain.Results.Data;
using VkBank.Domain.Entities;

namespace VkBank.Application.Features.Menu.Commands
{
    public class CreateMenuCommandRequest : IRequest<IResult>
    {
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

    public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommandRequest, IResult>
    {
        private readonly IMapper _mapper;
        private readonly CreateMenuValidator _validator;
        private readonly IMenuRepository _menuRepository;

        public CreateMenuCommandHandler(IMapper mapper, CreateMenuValidator validator, IMenuRepository menuRepository)
        {
            _mapper = mapper;
            _validator = validator;
            _menuRepository = menuRepository;
        }

        public async Task<IResult> Handle(CreateMenuCommandRequest request, CancellationToken cancellationToken)
        {
            if (request.ParentId != 0)
            {
                bool isParentIdExists = await _menuRepository.IsMenuParentIdExistsAsync(request.ParentId, cancellationToken);
                if (!isParentIdExists)
                {
                    return new ErrorResult(ResultMessages.MenuParentIdNotExist);
                }
            }

            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                string errorMessages = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                return new ErrorResult(errorMessages);
            }

            EntityMenu menu = _mapper.Map<EntityMenu>(request);

            long? menuId = await _menuRepository.CreateMenuAndGetIdAsync(menu, cancellationToken);
            if (menuId == null)
            {
                return new ErrorResult(ResultMessages.MenuCreateError);
            }

            return new SuccessDataResult<long?>(ResultMessages.MenuCreateSuccess, menuId);
        }
    }
}
