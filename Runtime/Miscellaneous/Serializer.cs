using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Z3.Utils.ExtensionMethods;
using Object = UnityEngine.Object;

namespace Z3.Utils
{
    public static class Serializer
    {
        private static readonly JsonSerializerSettings Settings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            ContractResolver = new WritablePropertiesOnlyResolver()
        };

        private static readonly JsonSerializerSettings ReadableSettings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented,
            ContractResolver = new WritablePropertiesOnlyResolver()
        };

        private static readonly UnitySerializablePropertyResolver UnitySerializable = new();

        private static JsonSerializerSettings CreateSettingsWithReferences(Type type, List<Object> refs) => new()
        {
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.None, // None because is impossible to read in a string
            Converters = new List<JsonConverter> { new UnityObjectIndexConverter(type, refs) },
            ContractResolver = UnitySerializable
        };

        private static readonly JsonSerializer FromJsonSerializer;
        private static readonly JsonSerializer ToJsonSerializer;

        static Serializer()
        {
            FromJsonSerializer = JsonSerializer.CreateDefault(Settings);
            ToJsonSerializer = JsonSerializer.CreateDefault(ReadableSettings);
        }

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
            using StringReader stringReader = new(data);
            using CustomReader jsonReader = new(stringReader);

            return FromJsonSerializer.Deserialize<T>(jsonReader);
        }

        public static T FromReadableJson<T>(string data)
        {
            using StringReader stringReader = new(data);
            using CustomReader jsonReader = new(stringReader);

            return ToJsonSerializer.Deserialize<T>(jsonReader);
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

            using StringReader stringReader = new(data);
            using CustomReader jsonReader = new(stringReader);
            return JsonSerializer.CreateDefault(settings).Deserialize(jsonReader, type);
        }

        /// <summary>
        /// Used to serialize writable fields, example Vector3
        /// </summary>
        private class WritablePropertiesOnlyResolver : DefaultContractResolver
        {
            /// <summary>
            /// Force read values
            /// </summary>
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
                return props.Where(p => p.Readable && p.Writable).ToList();
            }
        }

        /// <summary>
        /// If Unity can serialize, you can convert to Json
        /// </summary>
        private class UnitySerializablePropertyResolver : DefaultContractResolver
        {
            /// <summary>
            /// Force read values
            /// </summary>
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
                return props.Where(p => p.Readable && p.Writable).ToList();
            }

            /// <summary>
            /// Force write values (and read as well?)
            /// </summary>
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty prop = base.CreateProperty(member, memberSerialization);

                if (member is PropertyInfo propertyInfo)
                {
                    FieldInfo backingField = propertyInfo.GetBackingField();

                    bool hasSerializeField =
                        backingField != null &&
                        backingField.GetCustomAttribute<SerializeField>() != null;

                    if (hasSerializeField)
                    {
                        MethodInfo getter = propertyInfo.GetGetMethod(true);
                        MethodInfo setter = propertyInfo.GetSetMethod(true);

                        if (getter != null && setter != null)
                        {
                            prop.Readable = true;
                            prop.Writable = true;
                        }
                    }
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
                // TODO: Review long and double deserialization
                bool result = base.Read();

                if (TokenType == JsonToken.Integer)
                {
                    long longValue = (long)Value;
                    SetToken(JsonToken.Integer, (int)longValue, false);
                }
                else if (TokenType == JsonToken.Float)
                {
                    double doubleValue = (double)Value;
                    SetToken(JsonToken.Float, (float)doubleValue, false);
                }

                return result;
            }
        }

        /// <summary>
        /// Used to serialize UnityEngine.Object references as index based in a list. This list must to be serialized by Unity
        /// </summary>
        public sealed class UnityObjectIndexConverter : JsonConverter
        {
            private readonly List<Object> table;

            private readonly Action<JsonWriter> writeJsonFunc;
            private readonly Func<JsonReader, object> readJsonFunc;

            public UnityObjectIndexConverter(Type type, List<Object> table)
            {
                this.table = table;
                bool forceObject = CanConvert(type); // If root type is Object, we will force to write $ObjectReference

                if (forceObject)
                {
                    readJsonFunc = ReadJsonForce;
                    writeJsonFunc = WriteJsonForce;
                }
                else
                {
                    readJsonFunc = ReadJsonDefault;
                    writeJsonFunc = WriteJsonDefault;
                }
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

                writeJsonFunc(w);
            }

            public override object ReadJson(JsonReader r, Type t, object existingValue, JsonSerializer s)
            {
                return readJsonFunc(r);
            }

            private void WriteJsonForce(JsonWriter w)
            {
                // If you are serializing a Object this will be the fixed value

                // serializedValue: '"$ObjectReference"'
                w.WriteValue("$ObjectReference");
            }

            private void WriteJsonDefault(JsonWriter w)
            {
                // If you are serializing anything different than Object, it will serialize as JSON, but Objects will be indexes
                /* Examples
                
                1. Serialized class / struct
                 serializedValue: '{
                    "ProjectilePrefab":0, // Index of serializes object in list
                    "Speed":10.0,
                    "Damage":{
                        "HitMode":2,
                        "HitVfxs": [0, 1], // Indexes of serializes objects in list
                        "DamageRule":{
                            "Guid":"aaa111",
                            "Name":"Enemy"
                        },

                // 2. List<Transform>
                serializedValue: "[0,1,null,2,3,null]'
                */
                w.WriteValue(table.Count - 1);
            }

            private object ReadJsonForce(JsonReader r)
            {
                if (r.TokenType == JsonToken.Null)
                    return null;

                return table[0];
            }

            private object ReadJsonDefault(JsonReader r)
            {
                if (r.TokenType == JsonToken.Null)
                    return null;

                //if (r.TokenType != JsonToken.Integer) // TEMP: Safe operation
                //    throw new JsonSerializationException($"Expected integer or 'ref' token, but got {r.TokenType} instead.");

                int index = (int)r.Value;

                if (index < 0 || index >= table.Count) // TEMP: Safe operation
                {
                    Debug.LogError($"CRITICAL ERROR: You are trying to get an object outside of the table range: {index}");
                    return null;
                }

                return table[index];
            }
        }
    }
}
