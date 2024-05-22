namespace VkBank.Infrastructure.Services.Serialization.Abstract
{
    public interface ISerializerService
    {
        string Serialize(object obj);
        T? Deserialize<T>(string serializedObj);
    }
}
