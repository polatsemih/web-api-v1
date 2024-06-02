using Microsoft.AspNetCore.Http;

namespace SUPBank.Domain.Responses
{
    public class OkResponse : Response
    {
        public OkResponse(string message) : base(StatusCodes.Status200OK, message) { }
    }
}
