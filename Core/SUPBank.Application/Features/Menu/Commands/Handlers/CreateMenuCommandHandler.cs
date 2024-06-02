using AutoMapper;
using MediatR;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Entities;
using SUPBank.Domain.Responses;
using SUPBank.Domain.Responses.Data;
using SUPBank.Application.Features.Menu.Commands.Requests;

namespace SUPBank.Application.Features.Menu.Commands.Handlers
{
    public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommandRequest, IResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMenuQueryRepository _menuQueryRepository;
        private readonly IMenuCommandRepository _menuCommandRepository;

        public CreateMenuCommandHandler(IMapper mapper, IMenuQueryRepository menuQueryRepository, IMenuCommandRepository menuCommandRepository)
        {
            _mapper = mapper;
            _menuQueryRepository = menuQueryRepository;
            _menuCommandRepository = menuCommandRepository;
        }

        public async Task<IResponse> Handle(CreateMenuCommandRequest request, CancellationToken cancellationToken)
        {
            if (request.ParentId != 0 && !await _menuQueryRepository.IsParentIdExistsInMenuAsync(request.ParentId, cancellationToken))
            {
                return new BadRequestResponse(ResultMessages.MenuParentIdNotExist);
            }

            EntityMenu menu = _mapper.Map<EntityMenu>(request);

            long? menuId = await _menuCommandRepository.CreateMenuAndGetIdAsync(menu, cancellationToken);
            if (!menuId.HasValue)
            {
                return new InternalServerErrorResponse(ResultMessages.MenuCreateError);
            }

            return new OkDataResponse<long>(ResultMessages.MenuCreateSuccess, menuId.Value);
        }
    }
}
