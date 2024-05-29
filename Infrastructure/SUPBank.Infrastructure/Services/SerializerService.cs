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
            if (string.IsNullOrEmpty(serializedObj))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(serializedObj);
        }
    }
}
