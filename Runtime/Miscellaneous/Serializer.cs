using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Z3.Utils
{
    public static class Serializer
    {
        private static JsonSerializerSettings Settings => new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ContractResolver = new WritablePropertiesOnlyResolver()
        };

        private static JsonSerializerSettings ReadableSettings => new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented,
            ContractResolver = new WritablePropertiesOnlyResolver()
        };

        private static JsonSerializerSettings CreateSettingsWithReferences(Type type, List<Object> refs) => new()
        {
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.None, // None because is impossible to read in a string
            Converters = new List<JsonConverter> { new UnityObjectIndexConverter(type, refs) },
            ContractResolver = new WritablePropertiesOnlyResolver()
        };

        public static string ToJson<T>(T data)
        {
            return JsonConvert.SerializeObject(data, Settings);
        }

        public static string ToReadableJson<T>(T data)
        {
            return JsonConvert.SerializeObject(data, ReadableSettings);
        }

        public static object FromJson(string data) => FromJson<object>(data);

        public static T FromJson<T>(string data)
        {
            using StringReader stringReader = new StringReader(data);
            using CustomReader jsonReader = new CustomReader(stringReader);
            return JsonSerializer.CreateDefault(Settings).Deserialize<T>(jsonReader);
        }

        public static T FromReadableJson<T>(string data)
        {
            using StringReader stringReader = new StringReader(data);
            using CustomReader jsonReader = new CustomReader(stringReader);
            return JsonSerializer.CreateDefault(ReadableSettings).Deserialize<T>(jsonReader);
        }

        // Serialization with UnityEngine.Object references
        public static T FromJson<T>(string data, List<Object> refs) => (T)FromJson(data, typeof(T), refs);
        public static string ToJson<T>(T data, List<Object> refs) => ToJson(data, typeof(T), refs);
        public static string ToJson(object data, Type type, List<Object> refs)
        {
            refs.Clear(); // Clear to refresh references
            JsonSerializerSettings settings = CreateSettingsWithReferences(type, refs);
            return JsonConvert.SerializeObject(data, type, settings);
        }

        public static object FromJson(string data, Type type, List<Object> refs)
        {
            JsonSerializerSettings settings = CreateSettingsWithReferences(type, refs);

            using StringReader stringReader = new StringReader(data);
            using CustomReader jsonReader = new CustomReader(stringReader);
            return JsonSerializer.CreateDefault(settings).Deserialize(jsonReader, type);
        }

        /// <summary>
        /// Used to serialize only writable fields
        /// </summary>
        private class WritablePropertiesOnlyResolver : DefaultContractResolver
        {
            /// <summary>
            /// Force read values
            /// </summary>
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
                return props.Where(p => p.Readable || p.Writable).ToList();
            }

            /// <summary>
            /// Force write values (and read as well?)
            /// </summary>
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty prop = base.CreateProperty(member, memberSerialization);

                if (member is PropertyInfo propertyInfo && propertyInfo.GetCustomAttribute<SerializeField>() != null)
                {
                    MethodInfo getter = propertyInfo.GetGetMethod(true);
                    MethodInfo setter = propertyInfo.GetSetMethod(true);
                    if (getter != null && setter != null)
                    {
                        prop.Readable = true;
                        prop.Writable = true;
                    }

                    // Ensure we can read via non-public getter as well
                    //MethodInfo getter = propertyInfo.GetGetMethod(true);
                    //if (getter != null && prop.Readable == false)
                    //{
                    //    prop.Readable = true;
                    //}

                    //// If there is a non-public setter, allow writing
                    //MethodInfo setter = propertyInfo.GetSetMethod(true);
                    //if (setter != null && prop.Writable == false)
                    //{
                    //    prop.Writable = true;
                    //}
                }

                return prop;
            }
        }

        /// <summary>
        /// Used to give preference to deserialize numeric objects as Int instead Long
        /// </summary>
        /// <remarks>
        /// Source: https://stackoverflow.com/questions/8297541/how-do-i-change-the-default-type-for-numeric-deserialization
        /// </remarks>
        private class CustomReader : JsonTextReader
        {
            public CustomReader(TextReader reader) : base(reader) { }

            public override bool Read()
            {
                bool ret = base.Read();

                // TODO: Review long and double deserialization
                if (ValueType == typeof(long) && Value is long longValue)
                {
                    if (TokenType == JsonToken.Integer)
                    {
                        bool valueIsInsideBoundaries = longValue <= int.MaxValue && longValue >= int.MinValue;

                        if (valueIsInsideBoundaries)
                        {
                            int newValue = checked((int)longValue);
                            SetToken(TokenType, newValue, false);
                        }
                    }
                }
                else if (ValueType == typeof(double) && Value is double doubleValue)
                {
                    if (TokenType == JsonToken.Float)
                    {
                        bool valueIsInsideBoundaries = doubleValue <= float.MaxValue && doubleValue >= float.MinValue;

                        if (valueIsInsideBoundaries)
                        {
                            float newValue = (float)doubleValue;
                            SetToken(TokenType, newValue, false);
                        }
                    }
                }

                return ret;
            }
        }

        /// <summary>
        /// Used to serialize UnityEngine.Object references as index based in a list.
        /// </summary>
        public sealed class UnityObjectIndexConverter : JsonConverter
        {
            private readonly List<Object> table;
            private readonly bool forceObject;

            public UnityObjectIndexConverter(Type type, List<Object> table)
            {
                this.table = table;
                forceObject = CanConvert(type); // If root type is Object, we will force to write $ObjectReference
            }

            public override bool CanConvert(Type t) => typeof(Object).IsAssignableFrom(t);

            public override void WriteJson(JsonWriter w, object value, JsonSerializer s)
            {
                Object uobj = value as Object;
                if (ReferenceEquals(uobj, null) || uobj == null) 
                { 
                    w.WriteNull(); 
                    return; 
                }

                table.Add(uobj);
                if (forceObject)
                {
                    w.WriteValue("$ObjectReference");
                }
                else
                {
                    w.WriteValue(table.Count - 1);
                }
            }

            public override object ReadJson(JsonReader r, Type t, object existingValue, JsonSerializer s)
            {
                if (r.TokenType == JsonToken.Null) 
                    return null;

                if (r.TokenType != JsonToken.Integer && !forceObject)
                    throw new JsonSerializationException($"Expected integer or 'ref' token, but got {r.TokenType} instead.");

                int index = 0;
                if (r.TokenType == JsonToken.Integer)
                {
                    index = Convert.ToInt32(r.Value);
                }

                if (index < 0 || index >= table.Count)
                {
                    Debug.LogError($"CRITICAL ERROR: You are trying to get an object outside of the table range: {index}");
                    return null;
                }

                return table[index];

            }
        }
    }
}
