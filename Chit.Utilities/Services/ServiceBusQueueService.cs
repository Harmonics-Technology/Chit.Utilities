using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace Chit.Utilities;

public class ServiceBusQueueService : IServiceBusQueueService
{
    private readonly IConfiguration _config;

    public ServiceBusQueueService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendMessageAsync<T>(T serviceBusMessage, string queueName)
    {
        var queueClient = new ServiceBusClient(_config.GetSection("AppSettings:ServiceBusConnectionString").Value);
        var sender = queueClient.CreateSender(queueName);
        string messageBody = JsonSerializer.Serialize(serviceBusMessage);
        var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody));

        await sender.SendMessageAsync(message);
    }
}
