using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendGrid.Extensions.DependencyInjection;

namespace Chit.Utilities;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddUtilities(this IServiceCollection services, ConfigurationManager Configuration)
    {
        // TODO: Move notification settings to key vault
        services.AddTransient<IKeyVaultService, KeyVaultService>();
        services.AddAppSettings<NotificationConfig>(Configuration, "NotificationConfig");

        services.AddTransient<IServiceBusQueueService, ServiceBusQueueService>();
        services.AddTransient<INotificationHandler, NotificationHandler>();

        var config = Configuration.GetSection(nameof(NotificationConfig)).Get<NotificationConfig>();


        services.AddMvc(options =>
        {
            options.Filters.Add<LinkRewritingFilter>();
        });
        services.AddSendGrid(options =>
        {
            options.ApiKey = config.SendGridApiKey;
        });
        services.AddTransient<IEmailHandler, EmailHandler>();
        return services;
    }
}
