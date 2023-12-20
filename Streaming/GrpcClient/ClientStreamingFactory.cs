using Grpc.Core;
using Grpc.Net.Client;
using GrpcServer;
using System;

namespace GrpcClient
{
    public class ClientStreamingFactory
    {
        public static async Task CallDitto()
        {
            var source = new CancellationTokenSource();

            var channel = GrpcChannel.ForAddress("http://localhost:5232");
            var client = new StreamDemo.StreamDemoClient(channel);

            try
            {
                using AsyncDuplexStreamingCall<Msg, Msg>
                    stream = client.Speak(cancellationToken: source.Token);

                while (true)
                {
                    await WriteToStream(stream.RequestStream);

                    await ReadFromStream(stream.ResponseStream);
                                        
                    await stream.RequestStream.CompleteAsync();
                    Console.WriteLine("Client Complete.");
                }
            }
            catch (RpcException ex)
                when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Operation Cancelled");
            }
        }

        private static async Task ReadFromStream(
            IAsyncStreamReader<Msg> responseStream)
        {
            while (await responseStream.MoveNext())
            {
                Console.WriteLine(responseStream.Current.Text);
            }
        }

        private static async Task WriteToStream(
            IClientStreamWriter<Msg> requestStream)
        {
            for (var i = 0; i <= 10; i++)
            {
                await requestStream.WriteAsync(new Msg { Text = i.ToString() });
                await Task.Delay(1000);
            }
        }

        public static async Task BidirectionalStreamingDemo()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5232");
            var client = new StreamDemo.StreamDemoClient(channel);
            var stream = client.BidirectionalStreamingDemo();

            for (var i = 0; i < 10; i++)
            {
                var random = (new Random()).Next(1, 4);
                await stream.RequestStream.WriteAsync(new Test { TestMessage = i.ToString() });
                await Task.Delay(random * 1000);
                Console.WriteLine($"Send request: {i}");
            }

            await stream.RequestStream.CompleteAsync();

            //var requestTask = Task.Run(async () =>
            //{
            //    for (var i = 0; i < 10; i++)
            //    {
            //        var random = (new Random()).Next(1, 10);
            //        await Task.Delay(random);

            //        await stream.RequestStream
            //            .WriteAsync(new Test { TestMessage = i.ToString() });

            //        Console.WriteLine($"Send request: {i}");
            //    }

            //    await stream.RequestStream.CompleteAsync();
            //});

            while (await stream.ResponseStream.MoveNext(CancellationToken.None))
            {
                Console.WriteLine($"Received response: {stream.ResponseStream.Current.TestMessage}");
            }

            Console.WriteLine("Reponse stream completed");

            //var responseTask = Task.Run(async () =>
            //{
            //    while (await stream.ResponseStream.MoveNext(CancellationToken.None))
            //    {
            //        Console.WriteLine($"Received response: {stream.ResponseStream.Current.TestMessage}");
            //    }

            //    Console.WriteLine("Reponse stream completed");
            //});

            //await Task.WhenAll(requestTask, responseTask);
            await channel.ShutdownAsync();
        }

        public static async Task ClientStreamingDemo()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5232");
            var client = new StreamDemo.StreamDemoClient(channel);
            var stream = client.ClientStreamingDemo();

            for (int i = 0; i < 10; i++)
            {
                await stream.RequestStream.WriteAsync(new Test { TestMessage = $"Message {i}" });
                await Task.Delay(1000);
            }

            await stream.RequestStream.CompleteAsync();
            await channel.ShutdownAsync();

            Console.WriteLine("Completed client streaming");
        }

        public static async Task ServerStreamingDemoAsync()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5232");
            var client = new StreamDemo.StreamDemoClient(channel);
            var response = client.ServerStreamingDemo(new Test { TestMessage = "Test" });

            while (await response.ResponseStream.MoveNext(CancellationToken.None))
            {
                var value = response.ResponseStream.Current.TestMessage;
                Console.WriteLine(value);
            }

            Console.WriteLine("Server streaming Completed");
            await channel.ShutdownAsync();
        }
    }
}
