using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Utilities;
using ForkCommon.ExtensionMethods;

namespace Fork.Util.SwaggerUtils;

public class FriendlyStringEnumConverter : StringEnumConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        Enum e = (Enum)value;
        
        writer.WriteValue(e.FriendlyName());
    }
}