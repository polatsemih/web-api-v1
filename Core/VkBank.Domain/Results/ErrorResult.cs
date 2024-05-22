namespace VkBank.Domain.Results
{
    public class ErrorResult : Result
    {
        public ErrorResult(string? message) : base(false, message)
        {

        }
    }
}
