namespace SUPBank.Domain.Results.Data
{
    public interface IDataResult<T> : IResult
    {
        public T? Data { get; set; }
    }
}
