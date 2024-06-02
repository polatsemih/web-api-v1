using MediatR;
using SUPBank.Application.Features.Menu.Commands.Requests;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Responses;
using SUPBank.Domain.Responses.Data;

namespace SUPBank.Application.Features.Menu.Commands.Handlers
{
    public class RollbackMenusByTokenCommandHandler : IRequestHandler<RollbackMenusByTokenCommandRequest, IResponse>
    {
        private readonly IMenuQueryRepository _menuQueryRepository;
        private readonly IMenuCommandRepository _menuCommandRepository;

        public RollbackMenusByTokenCommandHandler(IMenuQueryRepository menuQueryRepository, IMenuCommandRepository menuCommandRepository)
        {
            _menuQueryRepository = menuQueryRepository;
            _menuCommandRepository = menuCommandRepository;
        }

        public async Task<IResponse> Handle(RollbackMenusByTokenCommandRequest request, CancellationToken cancellationToken)
        {
            if (!await _menuQueryRepository.IsRollbackTokenExistsInMenuHAsync(request.RollbackToken, cancellationToken))
            {
                return new BadRequestResponse(ResultMessages.MenuRollbackTokenNotExistInHistory);
            }

            int result = await _menuCommandRepository.RollbackMenusByTokenAsync(request.RollbackToken, cancellationToken);
            if (result > 0)
            {
                return new OkDataResponse<int>(ResultMessages.MenuRollbackSuccess, result);
            }
            return new OkResponse(ResultMessages.MenuRollbackNoChanges);
        }
    }
}
