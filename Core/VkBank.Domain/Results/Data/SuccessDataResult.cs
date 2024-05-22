namespace VkBank.Domain.Results.Data
{
    public class SuccessDataResult<T> : DataResult<T>
    {
        public SuccessDataResult(string message, T? data) : base(true, message, data)
        {

        }

        public SuccessDataResult(T? data) : base(true, data)
        {

        }

        public SuccessDataResult(string message) : base(true, message)
        {

        }
    }
}
