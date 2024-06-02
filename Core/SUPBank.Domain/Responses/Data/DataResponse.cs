namespace SUPBank.Domain.Responses.Data
{
    public class DataResponse<T> : Response, IDataResponse<T>
    {
        public T Data { get; set; }

        public DataResponse(int status, string message, T data) : base(status, message)
        {
            Data = data;
        }
    }
}
