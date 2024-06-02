using MediatR;
using SUPBank.Domain.Responses;
using System.ComponentModel.DataAnnotations;

namespace SUPBank.Application.Features.Menu.Queries.Requests
{
    public class GetMenuByIdQueryRequest : IRequest<IResponse>
    {
        [Range(1, long.MaxValue)]
        public long Id { get; set; }
    }
}
