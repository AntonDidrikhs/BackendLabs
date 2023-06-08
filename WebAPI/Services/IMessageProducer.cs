namespace WebAPI.Services
{
    public interface IMessageProducer
    {
        void SendMessage<T>(T message);
    }
}
