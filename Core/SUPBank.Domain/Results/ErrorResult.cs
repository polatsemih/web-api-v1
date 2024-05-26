namespace SUPBank.Domain.Results
{
    public class ErrorResult : Result
    {
        public ErrorResult(string? message) : base(false, message)
        {

        }
    }
}
