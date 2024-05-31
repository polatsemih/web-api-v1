using AutoMapper;
using MediatR;
using System.ComponentModel.DataAnnotations;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Application.Validations.Menu;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Entities;
using SUPBank.Domain.Results;

namespace SUPBank.Application.Features.Menu.Commands
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

        [Range(LengthLimits.MenuScreenCodeMinRange, int.MaxValue)]
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
        private readonly UpdateMenuValidator _validator;
        private readonly IMapper _mapper;
        private readonly IMenuQueryRepository _menuQueryRepository;
        private readonly IMenuCommandRepository _menuCommandRepository;

        public UpdateMenuCommandHandler(UpdateMenuValidator validator, IMapper mapper, IMenuQueryRepository menuQueryRepository, IMenuCommandRepository menuCommandRepository)
        {
            _validator = validator;
            _mapper = mapper;
            _menuQueryRepository = menuQueryRepository;
            _menuCommandRepository = menuCommandRepository;
        }

        public async Task<IResult> Handle(UpdateMenuCommandRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ErrorResult(string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage)));
            }

            if (!await _menuQueryRepository.IsIdExistsInMenuAsync(request.Id, cancellationToken))
            {
                return new ErrorResult(ResultMessages.MenuIdNotExist);
            }

            if (request.ParentId != 0 && !await _menuQueryRepository.IsParentIdExistsInMenuAsync(request.ParentId, cancellationToken))
            {
                return new ErrorResult(ResultMessages.MenuParentIdNotExist);
            }

            EntityMenu menu = _mapper.Map<EntityMenu>(request);

            int result = await _menuCommandRepository.UpdateMenuAsync(menu, cancellationToken);
            if (result > 0)
            {
                return new SuccessResult(ResultMessages.MenuUpdateSuccess);
            }
            else if (result == -1)
            {
                return new SuccessResult(ResultMessages.MenuUpdateNoChanges);
            }
            return new ErrorResult(ResultMessages.MenuUpdateError);
        }
    }
}
