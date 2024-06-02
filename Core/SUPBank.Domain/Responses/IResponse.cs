namespace SUPBank.Domain.Responses
{
    public interface IResponse
    {
        public int Status { get; }
        public string Message { get; }
    }
}
