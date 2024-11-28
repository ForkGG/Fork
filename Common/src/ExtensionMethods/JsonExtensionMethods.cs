using Newtonsoft.Json;

namespace ForkCommon.ExtensionMethods;

public static class JsonExtensionMethods
{
    /// <summary>
    ///     Serialize the supplied object to Json
    ///     If in DEBUG mode this json will be indented
    /// </summary>
    /// <param name="o">Object to serialize</param>
    /// <returns>Json representation of the supplied object</returns>
    public static string ToJson(this object o)
    {
        JsonSerializerSettings options = new()
        {
            TypeNameHandling = TypeNameHandling.All
        };
#if DEBUG
        //options.Formatting = Formatting.Indented;
        options.Formatting = Formatting.None;
#else
            options.Formatting = Formatting.None;
#endif
        return JsonConvert.SerializeObject(o, options);
    }

    /// <summary>
    ///     Deserialize a json object back to an object
    ///     THIS WILL THROW EXCEPTIONS VERY LIKELY
    /// </summary>
    /// <param name="json">Json representation of the object</param>
    /// <typeparam name="T">Type of the object</typeparam>
    /// <returns>Deserialized object</returns>
    public static T? FromJson<T>(this string json)
    {
        JsonSerializerSettings options = new()
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        return JsonConvert.DeserializeObject<T>(json, options);
    }
}