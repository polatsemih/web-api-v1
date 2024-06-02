using MediatR;
using SUPBank.Domain.Responses;
using System.ComponentModel.DataAnnotations;

namespace SUPBank.Application.Features.Menu.Commands.Requests
{
    public class RollbackMenuByIdCommandRequest : IRequest<IResponse>
    {
        [Range(1, long.MaxValue)]
        public required long Id { get; set; }
    }
}
