using MediatR;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Domain.Contstants;
using VkBank.Domain.Entities;
using VkBank.Domain.Results.Common;

namespace VkBank.Application.Features.Queries.GetEvent
{
    public class GetMenuQueryRequest : IRequest<IDataResult<Menu>>
    {
        public long Id { get; set; }
    }

    public class GetMenuQueryHandler : IRequestHandler<GetMenuQueryRequest, IDataResult<Menu>>
    {
        private readonly IMenuRepository _menuRepository;

        public GetMenuQueryHandler(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<IDataResult<Menu>> Handle(GetMenuQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _menuRepository.GetMenuByIdAsync(request.Id, cancellationToken);
            if (result == null)
            {
                return new ErrorDataResult<Menu>(null, ResultMessages.MenuNoData);
            }
            return new SuccessDataResult<Menu>(result);
        }
    }
}
