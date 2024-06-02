namespace SUPBank.Domain.Responses.Data
{
    public interface IDataResponse<T> : IResponse
    {
        public T Data { get; set; }
    }
}
