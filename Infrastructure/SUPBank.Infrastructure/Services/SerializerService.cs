using Newtonsoft.Json;
using SUPBank.Application.Interfaces.Services;

namespace SUPBank.Infrastructure.Services
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
