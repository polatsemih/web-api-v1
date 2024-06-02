using MediatR;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Responses;
using System.ComponentModel.DataAnnotations;

namespace SUPBank.Application.Features.Menu.Queries.Requests
{
    public class SearchMenuQueryRequest() : IRequest<IResponse>
    {
        [MinLength(LengthLimits.MenuKeywordMinLength)]
        [MaxLength(LengthLimits.MenuKeywordMaxLength)]
        public required string Keyword { get; set; }
    }
}
