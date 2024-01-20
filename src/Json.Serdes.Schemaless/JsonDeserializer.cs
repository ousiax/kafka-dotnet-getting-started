using Confluent.Kafka;

namespace Json.Serdes.Schemaless;

public class JsonDeserializer<T> : IAsyncDeserializer<T>, IDeserializer<T>
{
    public Task<T> DeserializeAsync(ReadOnlyMemory<byte> data, bool isNull, SerializationContext context)
    {
        T result = Deserialize(data.Span, isNull, context)!;
        return Task.FromResult(result);
    }
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(data)!;
    }
}
