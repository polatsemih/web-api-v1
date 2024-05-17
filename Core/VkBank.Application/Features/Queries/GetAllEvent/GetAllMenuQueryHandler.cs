using MediatR;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Domain.Contstants;
using VkBank.Domain.Entities;
using VkBank.Domain.Results.Common;

namespace VkBank.Application.Features.Queries.GetAllEvent
{
    public class GetAllMenuQueryRequest() : IRequest<IDataResult<List<Menu>>>
    {

    }

    public class GetAllMenuQueryHandler : IRequestHandler<GetAllMenuQueryRequest, IDataResult<List<Menu>>>
    {
        private readonly IMenuRepository _menuRepository;

        public GetAllMenuQueryHandler(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<IDataResult<List<Menu>>> Handle(GetAllMenuQueryRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<Menu> result = await _menuRepository.GetAllMenuAsync(cancellationToken);
            if (!result.Any())
            {
                return new ErrorDataResult<List<Menu>>(null, ResultMessages.MenuNoDatas);
            }
            return new SuccessDataResult<List<Menu>>(result.ToList());
        }
    }
}
