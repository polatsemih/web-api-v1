namespace VkBank.Domain.Results
{
    public class Result : IResult
    {
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }

        public Result(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public Result(bool success)
        {
            IsSuccess = success;
        }
    }
}
