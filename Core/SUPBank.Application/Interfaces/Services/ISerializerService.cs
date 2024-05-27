namespace SUPBank.Application.Interfaces.Services
{
    public interface ISerializerService
    {
        string Serialize(object obj);
        T? Deserialize<T>(string serializedObj);
    }
}
