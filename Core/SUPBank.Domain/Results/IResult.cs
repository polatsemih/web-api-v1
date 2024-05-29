namespace SUPBank.Domain.Results
{
    public interface IResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }
    }
}
