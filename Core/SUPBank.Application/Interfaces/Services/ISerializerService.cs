namespace SUPBank.Application.Interfaces.Services
{
    /// <summary>
    /// Defines methods for serializing and deserializing objects.
    /// </summary>
    public interface ISerializerService
    {
        /// <summary>
        /// Serializes an object into a string representation.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A string representing the serialized object.</returns>
        string Serialize(object obj);

        /// <summary>
        /// Deserializes a string representation into an object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="serializedObj">The string representation to deserialize.</param>
        /// <returns>The deserialized object, or null if the string representation is null or empty.</returns>
        T? Deserialize<T>(string serializedObj);
    }
}
