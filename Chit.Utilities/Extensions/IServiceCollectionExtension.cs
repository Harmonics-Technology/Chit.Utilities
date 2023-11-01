using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendGrid.Extensions.DependencyInjection;

namespace Chit.Utilities;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddUtilities(this IServiceCollection services, ConfigurationManager Configuration)
    {
        var notificationSettingsSection = Configuration.GetSection("NotificationConfig");
        services.Configure<NotificationConfig>(notificationSettingsSection);

        var config = notificationSettingsSection.Get<NotificationConfig>();

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
