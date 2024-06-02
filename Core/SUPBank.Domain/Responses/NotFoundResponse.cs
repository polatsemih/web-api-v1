using Microsoft.AspNetCore.Http;

namespace SUPBank.Domain.Responses
{
    public class NotFoundResponse : Response
    {
        public NotFoundResponse(string message) : base(StatusCodes.Status404NotFound, message) { }
    }
}
