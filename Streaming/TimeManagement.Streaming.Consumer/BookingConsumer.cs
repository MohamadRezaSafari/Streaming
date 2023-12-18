using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace TimeManagement.Streaming.Consumer
{
    internal class BookingConsumer : IBookingConsumer
    {
        public void Listen(Action<string> message)
        {
            //var config = new Dictionary<string, object>
            //{
            //    {"group.id", "booking_consumer" },
            //    {"bootstrap.servers", "localhost:9092" },
            //    {"enable.auto.commit", "false" }
            //};

            //new ConsumerBuilder<Null, string>()

            //using var consumer =
            //    new Consumer<Null, string>(config, null, new StringDeserializer(Encoding.UTF8));


            var config = new ConsumerConfig
            {
                GroupId = "booking_consumer",
                BootstrapServers = "localhost:9092",
                EnableAutoCommit = false
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

            consumer.Subscribe("timemanagement_booking");

            while (true)
            {
                var consumeResult = consumer.Consume();

                //consumeResult.Message
            }
            
            consumer.Close();
        }
    }
}
