using MediatR;
using SUPBank.Domain.Responses;

namespace SUPBank.Application.Features.Menu.Commands.Requests
{
    public class RollbackMenusByTokenCommandRequest : IRequest<IResponse>
    {
        public required Guid RollbackToken { get; set; }
    }
}
