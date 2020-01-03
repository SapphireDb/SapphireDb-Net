using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SapphireDb_Net.Command;

namespace SapphireDb_Net.Helper
{
    public static class JsonHelper
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        
        public static readonly JsonSerializerSettings DeserializeResponseSettings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = { new CustomJsonConverter<ResponseBase>() }
        };
        
        public static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, Settings);
        }
        
        public static object Deserialize(string value, Type t)
        {
            try
            {
                return JsonConvert.DeserializeObject(value, t, Settings);
            }
            catch
            {
                return null;
            }
        }
        
        public static ResponseBase DeserializeResponse(string value)
        {
            try
            {
                return JsonConvert.DeserializeObject<ResponseBase>(value, DeserializeResponseSettings);
            }
            catch
            {
                return null;
            }
        }
    }
    
    class CustomJsonConverter<T> : JsonConverter
    {
        private readonly Dictionary<string, Type> nameTypeMappings = new Dictionary<string, Type>();

        public CustomJsonConverter()
        {
            nameTypeMappings = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(ResponseBase).IsAssignableFrom(t) && t.Name.EndsWith("Response"))
                .ToDictionary(t => t.Name, t => t);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);

            string key = "responseType";

            string typeString = jObject[key].Value<string>();

            if (nameTypeMappings.ContainsKey(typeString))
            {
                return jObject.ToObject(nameTypeMappings[typeString], serializer);
            }

            return null;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}