using Newtonsoft.Json;
using VkBank.Infrastructure.Services.Serialization.Abstract;

namespace VkBank.Infrastructure.Services.Serialization.Concrete
{
    public class SerializerService : ISerializerService
    {
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public T? Deserialize<T>(string serializedObj)
        {
            return JsonConvert.DeserializeObject<T>(serializedObj);
        }
    }
}
