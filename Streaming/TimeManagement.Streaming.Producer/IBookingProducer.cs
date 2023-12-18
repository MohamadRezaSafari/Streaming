namespace TimeManagement.Streaming.Producer
{
    internal interface IBookingProducer
    {
        Task ProduceAsync(string message);
    }
}
