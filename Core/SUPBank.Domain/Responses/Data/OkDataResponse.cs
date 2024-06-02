using Microsoft.AspNetCore.Http;

namespace SUPBank.Domain.Responses.Data
{
    public class OkDataResponse<T> : DataResponse<T>
    {
        public OkDataResponse(string message, T data) : base(StatusCodes.Status200OK, message, data) { }
        public OkDataResponse(T data) : base(StatusCodes.Status200OK, string.Empty, data) { }
    }
}
