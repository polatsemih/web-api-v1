namespace SUPBank.Domain.Results
{
    public class Result : IResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }

        public Result(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }
}
