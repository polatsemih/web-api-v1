namespace SUPBank.Domain.Results
{
    public interface IResult
    {
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}
