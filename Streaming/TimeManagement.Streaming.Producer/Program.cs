
using TimeManagement.Streaming.Producer;



var producer = new Producer<Message>();

for (int i = 0; i < int.MaxValue; i++)
{
    await producer.ProduceAsync(new Message
    {
        Data = $"Pushing Data {i} !!",
    });

    await Task.Delay(1000);
}

//Console.WriteLine("Publish Success!");
//Console.ReadKey();