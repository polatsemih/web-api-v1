using MediatR;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Domain.Contstants;
using VkBank.Domain.Entities;
using VkBank.Domain.Results;
using VkBank.Domain.Results.Data;

namespace VkBank.Application.Features.Menu.Queries
{
    public class GetAllMenuQueryRequest() : IRequest<IDataResult<List<EntityMenu>>>
    {

    }

    public class GetAllMenuQueryHandler : IRequestHandler<GetAllMenuQueryRequest, IDataResult<List<EntityMenu>>>
    {
        private readonly IMenuRepository _menuRepository;

        public GetAllMenuQueryHandler(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<IDataResult<List<EntityMenu>>> Handle(GetAllMenuQueryRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<EntityMenu> result = await _menuRepository.GetAllMenuAsync(cancellationToken);
            if (!result.Any())
            {
                return new ErrorDataResult<List<EntityMenu>>(ResultMessages.MenuNoDatas);
            }
            return new SuccessDataResult<List<EntityMenu>>(result.ToList());
        }
    }
}
