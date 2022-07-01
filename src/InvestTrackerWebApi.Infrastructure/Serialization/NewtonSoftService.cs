namespace InvestTrackerWebApi.Infrastructure.Serialization;
using InvestTrackerWebApi.Application.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

public class NewtonSoftService : ISerializerService
{
    public T Deserialize<T>(string text) => JsonConvert.DeserializeObject<T>(text)!;

    public string Serialize<T>(T obj) => JsonConvert.SerializeObject(obj, new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Converters = new List<JsonConverter>
        {
            new StringEnumConverter { NamingStrategy = new DefaultNamingStrategy() }
        }
    });

    public string Serialize<T>(T obj, Type type) => JsonConvert.SerializeObject(obj, type, new());
}

