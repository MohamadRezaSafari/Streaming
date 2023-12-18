using Confluent.Kafka;

namespace TimeManagement.Streaming.Producer
{
    internal class BookingProducer : IBookingProducer
    {
        public async Task ProduceAsync(string message)
        {
            var config = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092"
            };
            var producer = new ProducerBuilder<Null, string>(config).Build();

            await producer.ProduceAsync("timemanagement_booking", new Message<Null, string>()
            {
                Value = message
            });

            producer.Flush(TimeSpan.FromSeconds(10));
        }
    }
}
