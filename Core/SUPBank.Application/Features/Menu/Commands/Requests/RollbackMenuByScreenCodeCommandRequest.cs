using MediatR;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Responses;
using System.ComponentModel.DataAnnotations;

namespace SUPBank.Application.Features.Menu.Commands.Requests
{
    public class RollbackMenuByScreenCodeCommandRequest : IRequest<IResponse>
    {
        [Range(LengthLimits.MenuScreenCodeMinRange, int.MaxValue)]
        public required int ScreenCode { get; set; }
    }
}
