using AutoMapper;
using MediatR;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Entities;
using SUPBank.Domain.Responses;
using SUPBank.Application.Features.Menu.Commands.Requests;

namespace SUPBank.Application.Features.Menu.Commands.Handlers
{
    public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommandRequest, IResponse>
    {
        private readonly IMapper _mapper;
        private readonly IMenuQueryRepository _menuQueryRepository;
        private readonly IMenuCommandRepository _menuCommandRepository;

        public UpdateMenuCommandHandler(IMapper mapper, IMenuQueryRepository menuQueryRepository, IMenuCommandRepository menuCommandRepository)
        {
            _mapper = mapper;
            _menuQueryRepository = menuQueryRepository;
            _menuCommandRepository = menuCommandRepository;
        }

        public async Task<IResponse> Handle(UpdateMenuCommandRequest request, CancellationToken cancellationToken)
        {
            if (!await _menuQueryRepository.IsIdExistsInMenuAsync(request.Id, cancellationToken))
            {
                return new BadRequestResponse(ResultMessages.MenuIdNotExist);
            }

            if (request.ParentId != 0 && !await _menuQueryRepository.IsParentIdExistsInMenuAsync(request.ParentId, cancellationToken))
            {
                return new BadRequestResponse(ResultMessages.MenuParentIdNotExist);
            }

            EntityMenu menu = _mapper.Map<EntityMenu>(request);

            int result = await _menuCommandRepository.UpdateMenuAsync(menu, cancellationToken);
            if (result > 0)
            {
                return new OkResponse(ResultMessages.MenuUpdateSuccess);
            }
            return new OkResponse(ResultMessages.MenuUpdateNoChanges);
        }
    }
}
