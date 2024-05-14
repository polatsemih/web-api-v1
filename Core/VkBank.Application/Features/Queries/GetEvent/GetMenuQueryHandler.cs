using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Domain.Common.Result;
using VkBank.Domain.Entities;

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

        public Task<IDataResult<Menu>> Handle(GetMenuQueryRequest request, CancellationToken cancellationToken)
        {
            var result = _menuRepository.GetById(request.Id);
            return Task.FromResult<IDataResult<Menu>>(new SuccessDataResult<Menu>(result));
        }
    }
}
