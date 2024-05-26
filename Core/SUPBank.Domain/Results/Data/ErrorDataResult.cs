namespace SUPBank.Domain.Results.Data
{
    public class ErrorDataResult<T> : DataResult<T>
    {
        public ErrorDataResult(string message, T? data) : base(false, message, data)
        {

        }

        public ErrorDataResult(T? data) : base(false, data)
        {

        }

        public ErrorDataResult(string message) : base(false, message)
        {

        }
    }
}
