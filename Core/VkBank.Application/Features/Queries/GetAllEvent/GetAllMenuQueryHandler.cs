using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Domain.Common.Result;
using VkBank.Domain.Entities;

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

        public Task<IDataResult<List<Menu>>> Handle(GetAllMenuQueryRequest request, CancellationToken cancellationToken)
        {
            var result = _menuRepository.GetAll();
            return Task.FromResult<IDataResult<List<Menu>>>(new SuccessDataResult<List<Menu>>(result));
        }
    }
}
