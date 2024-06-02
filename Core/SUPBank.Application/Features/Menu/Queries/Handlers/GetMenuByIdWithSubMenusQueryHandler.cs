using MediatR;
using Microsoft.IdentityModel.Tokens;
using SUPBank.Application.Features.Menu.Queries.Requests;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Entities;
using SUPBank.Domain.Responses;
using SUPBank.Domain.Responses.Data;

namespace SUPBank.Application.Features.Menu.Queries.Handlers
{
    public class GetMenuByIdWithSubMenusQueryHandler : IRequestHandler<GetMenuByIdWithSubMenusQueryRequest, IResponse>
    {
        private readonly IMenuQueryRepository _menuQueryRepository;

        public GetMenuByIdWithSubMenusQueryHandler(IMenuQueryRepository menuQueryRepository)
        {
            _menuQueryRepository = menuQueryRepository;
        }

        public async Task<IResponse> Handle(GetMenuByIdWithSubMenusQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _menuQueryRepository.GetMenuByIdWithSubMenusAsync(request.Id, cancellationToken);
            if (result.IsNullOrEmpty())
            {
                return new NotFoundResponse(ResultMessages.MenuNoData);
            }
            return new OkDataResponse<List<EntityMenu>>(result.ToList());
        }
    }
}
