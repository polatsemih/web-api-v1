using MediatR;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using SUPBank.Domain.Responses;
using SUPBank.Domain.Responses.Data;
using SUPBank.Application.Features.Menu.Queries.Requests;

namespace SUPBank.Application.Features.Menu.Queries.Handlers
{
    public class SearchMenuQueryHandler : IRequestHandler<SearchMenuQueryRequest, IResponse>
    {
        private readonly IMenuQueryRepository _menuQueryRepository;

        public SearchMenuQueryHandler(IMenuQueryRepository menuQueryRepository)
        {
            _menuQueryRepository = menuQueryRepository;
        }

        public async Task<IResponse> Handle(SearchMenuQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _menuQueryRepository.SearchMenusAsync(request.Keyword, cancellationToken);
            if (result.IsNullOrEmpty())
            {
                return new NotFoundResponse(ResultMessages.MenuNoDatas);
            }
            return new OkDataResponse<List<EntityMenu>>(result.ToList());
        }
    }
}
