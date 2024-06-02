using Microsoft.AspNetCore.Http;

namespace SUPBank.Domain.Responses
{
    public class InternalServerErrorResponse : Response
    {
        public InternalServerErrorResponse(string message) : base(StatusCodes.Status500InternalServerError, message) { }
    }
}
