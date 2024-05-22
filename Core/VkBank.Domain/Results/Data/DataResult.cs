namespace VkBank.Domain.Results.Data
{
    public class DataResult<T> : Result, IDataResult<T>
    {
        public T? Data { get; set; }

        public DataResult(bool isSuccess, string message, T? data) : base(isSuccess, message)
        {
            Data = data;
        }

        public DataResult(bool isSuccess, T? data) : base(isSuccess)
        {
            Data = data;
        }

        public DataResult(bool isSuccess, string message) : base(isSuccess, message)
        {

        }
    }
}
