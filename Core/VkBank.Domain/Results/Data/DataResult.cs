namespace VkBank.Domain.Results.Data
{
    public class DataResult<T> : Result, IDataResult<T>
    {
        public T? Data { get; set; }

        public DataResult(bool suucess, string message, T? data) : base(suucess, message)
        {
            Data = data;
        }

        public DataResult(bool success, T? data) : base(success)
        {
            Data = data;
        }
    }
}
