using Confluent.Kafka;

namespace Json.Serdes.Schemaless;

public class JsonSerializer<T> : IAsyncSerializer<T>, ISerializer<T>
{
    public Task<byte[]> SerializeAsync(T data, SerializationContext context)
    {
        return Task.FromResult(Serialize(data, context));
    }

    public byte[] Serialize(T data, SerializationContext context)
    {
        return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(data);
    }
}
