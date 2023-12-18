using Confluent.Kafka;
using System.Text.Json;

namespace TimeManagement.Streaming.Consumer
{
    public class CustomValueDeserializer<T> : IDeserializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context) 
            => JsonSerializer.Deserialize<T>(data);
    }
}
