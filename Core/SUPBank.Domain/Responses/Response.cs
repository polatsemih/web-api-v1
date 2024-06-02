namespace SUPBank.Domain.Responses
{
    public class Response : IResponse
    {
        public int Status { get; }
        public string Message { get; }

        public Response(int status, string message)
        {
            Status = status;
            Message = message;
        }
    }
}
