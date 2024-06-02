using MediatR;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Responses;
using SUPBank.Application.Features.Menu.Commands.Requests;

namespace SUPBank.Application.Features.Menu.Commands.Handlers
{
    public class RollbackMenuByScreenCodeCommandHandler : IRequestHandler<RollbackMenuByScreenCodeCommandRequest, IResponse>
    {
        private readonly IMenuQueryRepository _menuQueryRepository;
        private readonly IMenuCommandRepository _menuCommandRepository;

        public RollbackMenuByScreenCodeCommandHandler(IMenuQueryRepository menuQueryRepository, IMenuCommandRepository menuCommandRepository)
        {
            _menuQueryRepository = menuQueryRepository;
            _menuCommandRepository = menuCommandRepository;
        }

        public async Task<IResponse> Handle(RollbackMenuByScreenCodeCommandRequest request, CancellationToken cancellationToken)
        {
            if (!await _menuQueryRepository.IsScreenCodeExistsInMenuHAsync(request.ScreenCode, cancellationToken))
            {
                return new BadRequestResponse(ResultMessages.MenuScreenCodeNotExistInHistory);
            }

            int result = await _menuCommandRepository.RollbackMenuByScreenCodeAsync(request.ScreenCode, cancellationToken);
            if (result == 1)
            {
                return new OkResponse(ResultMessages.MenuRollbackSuccess);
            }
            return new OkResponse(ResultMessages.MenuRollbackNoChanges);
        }
    }
}
