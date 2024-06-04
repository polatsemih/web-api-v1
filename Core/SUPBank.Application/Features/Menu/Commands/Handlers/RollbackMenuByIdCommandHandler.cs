using MediatR;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Responses;
using SUPBank.Application.Features.Menu.Commands.Requests;

namespace SUPBank.Application.Features.Menu.Commands.Handlers
{
    public class RollbackMenuByIdCommandHandler : IRequestHandler<RollbackMenuByIdCommandRequest, IResponse>
    {
        private readonly IMenuQueryRepository _menuQueryRepository;
        private readonly IMenuCommandRepository _menuCommandRepository;

        public RollbackMenuByIdCommandHandler(IMenuQueryRepository menuQueryRepository, IMenuCommandRepository menuCommandRepository)
        {
            _menuQueryRepository = menuQueryRepository;
            _menuCommandRepository = menuCommandRepository;
        }

        public async Task<IResponse> Handle(RollbackMenuByIdCommandRequest request, CancellationToken cancellationToken)
        {
            if (!await _menuQueryRepository.IsMenuIdExistsInMenuHAsync(request.Id, cancellationToken))
            {
                return new BadRequestResponse(ResultMessages.MenuIdNotExistInHistory);
            }

            if (!await _menuQueryRepository.IsIdExistsInMenuAsync(request.Id, cancellationToken))
            {
                var menuH = await _menuQueryRepository.GetMenuByIdInMenuHAsync(request.Id, cancellationToken);
                if (menuH != null && menuH.ScreenCode.HasValue && await _menuQueryRepository.IsScreenCodeExistsInMenuAsync(menuH.ScreenCode.Value, cancellationToken))
                {
                    return new BadRequestResponse(ResultMessages.MenuScreenCodeAlreadyExistsInOriginalTable);
                }
            }

            int result = await _menuCommandRepository.RollbackMenuByIdAsync(request.Id, cancellationToken);
            if (result == 1)
            {
                return new OkResponse(ResultMessages.MenuRollbackSuccess);
            }
            return new OkResponse(ResultMessages.MenuRollbackNoChanges);
        }
    }
}
