namespace Chit.Utilities;

public interface IServiceBusQueueService
{
    Task SendMessageAsync<T>(T serviceBusMessage, string queueName);
}
