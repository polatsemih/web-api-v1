namespace SUPBank.Domain.Results.Data
{
    public class SuccessDataResult<T> : DataResult<T>
    {
        public SuccessDataResult(string message, T data) : base(true, message, data)
        {

        }

        public SuccessDataResult(string message) : base(true, message)
        {

        }

        public SuccessDataResult(T data) : base(true, string.Empty, data)
        {

        }
    }
}
