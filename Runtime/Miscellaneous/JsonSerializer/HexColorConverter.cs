using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Z3.Utils
{
    /// <summary>Serializes a <see cref="Color"/> as a "#RRGGBBAA" hex string.</summary>
    public class HexColorConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            writer.WriteValue("#" + ColorUtility.ToHtmlStringRGBA(value));
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return reader.Value is string hex && ColorUtility.TryParseHtmlString(hex, out Color color) ? color : existingValue;
        }
    }
}
