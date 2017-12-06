using System;
using Cars.Core;
using Cars.EventSource.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cars.Testing.Shared
{
    public class JsonTextSerializer : ITextSerializer
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public string Serialize(object @object)
        {
            return JsonConvert.SerializeObject(@object, _settings);
        }

        public object Deserialize(string textSerialized, string type)
        {
            var clrType = Type.GetType(type);

            if (clrType == null) throw new EventTypeNotFoundException(type);

            return JsonConvert.DeserializeObject(textSerialized, clrType, _settings);
        }

        public T Deserialize<T>(string textSerialized)
        {
            return JsonConvert.DeserializeObject<T>(textSerialized, _settings);
        }
    }
}