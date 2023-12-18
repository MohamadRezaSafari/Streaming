namespace TimeManagement.Streaming.Consumer
{
    internal interface IBookingConsumer
    {
        void Listen(Action<string> message);
    }
}
