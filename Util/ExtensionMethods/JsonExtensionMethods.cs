using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectAvery.Util.ExtensionMethods
{
    public static class JsonExtensionMethods
    {
        /// <summary>
        /// Serialize the supplied object to Json
        /// If in DEBUG mode this json will be indented
        /// </summary>
        /// <param name="o">Object to serialize</param>
        /// <returns>Json representation of the supplied object</returns>
        public static string ToJson(this object o)
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
#if DEBUG
            options.WriteIndented = true;
#endif
            return JsonSerializer.Serialize(o, options);
        }

        /// <summary>
        /// Deserialize a json object back to an object
        /// </summary>
        /// <param name="json">Json representation of the object</param>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <returns>Deserialized object</returns>
        public static T FromJson<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}