using MediatR;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Responses;
using SUPBank.Application.Features.Menu.Commands.Requests;

namespace SUPBank.Application.Features.Menu.Commands.Handlers
{
    public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommandRequest, IResponse>
    {
        private readonly IMenuQueryRepository _menuQueryRepository;
        private readonly IMenuCommandRepository _menuCommandRepository;

        public DeleteMenuCommandHandler(IMenuQueryRepository menuQueryRepository, IMenuCommandRepository menuCommandRepository)
        {
            _menuQueryRepository = menuQueryRepository;
            _menuCommandRepository = menuCommandRepository;
        }

        public async Task<IResponse> Handle(DeleteMenuCommandRequest request, CancellationToken cancellationToken)
        {
            if (!await _menuQueryRepository.IsIdExistsInMenuAsync(request.Id, cancellationToken))
            {
                return new BadRequestResponse(ResultMessages.MenuIdNotExist);
            }

            return await _menuCommandRepository.DeleteMenuAsync(request.Id, cancellationToken) ?
                new OkResponse(ResultMessages.MenuDeleteSuccess) :
                new BadRequestResponse(ResultMessages.MenuDeleteError);
        }
    }
}
