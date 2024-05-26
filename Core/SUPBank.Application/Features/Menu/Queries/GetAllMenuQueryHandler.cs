using MediatR;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Entities;
using SUPBank.Domain.Results.Data;

namespace SUPBank.Application.Features.Menu.Queries
{
    public class GetAllMenuQueryRequest() : IRequest<IDataResult<List<EntityMenu>>>
    {

    }

    public class GetAllMenuQueryHandler : IRequestHandler<GetAllMenuQueryRequest, IDataResult<List<EntityMenu>>>
    {
        private readonly IMenuQueryRepository _menuQueryRepository;

        public GetAllMenuQueryHandler(IMenuQueryRepository menuQueryRepository)
        {
            _menuQueryRepository = menuQueryRepository;
        }

        public async Task<IDataResult<List<EntityMenu>>> Handle(GetAllMenuQueryRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<EntityMenu> result = await _menuQueryRepository.GetAllMenusAsync(cancellationToken);
            if (!result.Any())
            {
                return new ErrorDataResult<List<EntityMenu>>(ResultMessages.MenuNoDatas);
            }
            return new SuccessDataResult<List<EntityMenu>>(result.ToList());
        }
    }
}
