namespace VkBank.Domain.Results
{
    public class SuccessResult : Result
    {
        public SuccessResult(string? message) : base(true, message)
        {

        }
    }
}
