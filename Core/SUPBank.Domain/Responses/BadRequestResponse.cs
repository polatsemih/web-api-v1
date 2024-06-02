using Microsoft.AspNetCore.Http;

namespace SUPBank.Domain.Responses
{
    public class BadRequestResponse : Response
    {
        public BadRequestResponse(string message) : base(StatusCodes.Status400BadRequest, message) { }
    }
}
