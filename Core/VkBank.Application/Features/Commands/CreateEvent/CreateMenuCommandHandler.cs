using AutoMapper;
using MediatR;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Application.Validations.Create;
using VkBank.Domain.Contstants;
using VkBank.Domain.Entities;
using VkBank.Domain.Results.Common;

namespace VkBank.Application.Features.Commands.CreateEvent
{
    public class CreateMenuCommandRequest : IRequest<IResult>
    {
        public long ParentId { get; set; }
        public string Name_TR { get; set; }
        public string Name_EN { get; set; }
        public int ScreenCode { get; set; }
        public byte Type { get; set; }
        public int Priority { get; set; }
        public string Keyword { get; set; }
        public string? Icon { get; set; }
        public bool IsGroup { get; set; }
        public bool IsNew { get; set; }
        public DateTime? NewStartDate { get; set; }
        public DateTime? NewEndDate { get; set; }
        public bool IsActive { get; set; }
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
            Menu menu = _mapper.Map<Menu>(request);

            var validationResult = _validator.Validate(menu);
            if (!validationResult.IsValid)
            {
                return new ErrorResult(validationResult.Errors.First().ErrorMessage);
            }

            long? menuId = await _menuRepository.CreateMenuAndGetIdAsync(menu, cancellationToken);
            if (menuId == null)
            {
                return new ErrorResult(ResultMessages.MenuCreatedFailed);
            }

            return new SuccessDataResult<long?>(menuId, ResultMessages.MenuCreated);
        }
    }
}
