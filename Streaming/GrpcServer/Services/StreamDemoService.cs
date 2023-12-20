using Grpc.Core;

namespace GrpcServer.Services
{
    public class StreamDemoService : StreamDemo.StreamDemoBase
    {
        public override async Task Speak(
            IAsyncStreamReader<Msg> requestStream,
            IServerStreamWriter<Msg> responseStream,
            ServerCallContext context)
        {
            try
            {
                while (await requestStream.MoveNext()
                  && !context.CancellationToken.IsCancellationRequested)
                {
                    var current = requestStream.Current;
                    Console.WriteLine($"Message from Client: {current.Text}");

                    await SendResponseMessage(current, responseStream);
                }
            }
            catch (RpcException ex) 
                when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Operation Cancelled");
            }

            Console.WriteLine("Operation Complete.");
        }

        private async Task SendResponseMessage(
            Msg current,
            IServerStreamWriter<Msg> responseStream)
        {
            await responseStream.WriteAsync(new Msg
            {
                Text = $"From Server: {current.Text} - {Guid.NewGuid()}"
            });
        }

        public override async Task BidirectionalStreamingDemo(
            IAsyncStreamReader<Test> requestStream,
            IServerStreamWriter<Test> responseStream,
            ServerCallContext context)
        {
            //var tasks = new List<Task>();

            while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested)
            {
                //Console.WriteLine($"Received request: {requestStream.Current.TestMessage}");

                if (!string.IsNullOrEmpty(requestStream.Current.TestMessage))
                {
                        var message = requestStream.Current.TestMessage;
                        Console.WriteLine($"Message from client: {message}");
                   

                    //var task = Task.Run(async () =>
                    //{
                    //    var message = requestStream.Current.TestMessage;
                    //    var random = (new Random()).Next(1, 10);
                    //    await Task.Delay(random);

                    //    //await responseStream.WriteAsync(new Test { TestMessage = message });
                    //    //Console.WriteLine($"Sent response: {message}");
                    //});
                    //tasks.Add(task);
                }
            }

            while (!context.CancellationToken.IsCancellationRequested)
            {
                    var message = Guid.NewGuid().ToString();
                    var random = (new  Random()).Next(1,4);
                    await responseStream.WriteAsync(new Test { TestMessage = message });
                    Console.WriteLine($"Sent response: {message}");
                    await Task.Delay(1000*random);
                

                //var task = new Task(async () =>
                //{
                //    await responseStream.WriteAsync(new Test { TestMessage = message });
                //    Console.WriteLine($"Sent response: {message}");
                //});
                //tasks.Add(task);
            }

            //await Task.WhenAll(tasks);
            Console.WriteLine("Bidirectional streaming completed");
        }

        public override async Task<Test> ClientStreamingDemo(
            IAsyncStreamReader<Test> requestStream,
            ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                Console.WriteLine(requestStream.Current.TestMessage);
            }

            Console.WriteLine("Client streaming completed");
            return new Test() { TestMessage = "Sample" };
        }

        public override async Task ServerStreamingDemo(
            Test request,
            IServerStreamWriter<Test> responseStream,
            ServerCallContext context)
        {
            for (int i = 0; i < 20; i++)
            {
                await responseStream
                    .WriteAsync(new Test { TestMessage = $"Message {i}" });
                var randomNumber = (new Random()).Next(1, 10);
                await Task.Delay(randomNumber * 1000);
            }
        }
    }
}
