using MediatR;
using SUPBank.Application.Features.Menu.Queries.Requests;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Entities;
using SUPBank.Domain.Responses;
using SUPBank.Domain.Responses.Data;

namespace SUPBank.Application.Features.Menu.Queries.Handlers
{
    public class GetMenuByIdQueryHandler : IRequestHandler<GetMenuByIdQueryRequest, IResponse>
    {
        private readonly IMenuQueryRepository _menuQueryRepository;

        public GetMenuByIdQueryHandler(IMenuQueryRepository menuQueryRepository)
        {
            _menuQueryRepository = menuQueryRepository;
        }

        public async Task<IResponse> Handle(GetMenuByIdQueryRequest request, CancellationToken cancellationToken)
        {
            if (!await _menuQueryRepository.IsIdExistsInMenuAsync(request.Id, cancellationToken))
            {
                return new BadRequestResponse(ResultMessages.MenuIdNotExist);
            }

            var result = await _menuQueryRepository.GetMenuByIdAsync(request.Id, cancellationToken);
            if (result == null)
            {
                return new NotFoundResponse(ResultMessages.MenuNoData);
            }
            return new OkDataResponse<EntityMenu>(result);
        }
    }
}
