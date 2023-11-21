using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chit.Utilities;

public static class AppSettingsGenerationsExtension
{
    public static IServiceCollection AddAppSettings<T>(this IServiceCollection services, IConfiguration configuration, string config) where T : class
    {
        // ger required service of type IKeyVaultService
        var keyVaultService = services.BuildServiceProvider().GetRequiredService<IKeyVaultService>();
        var section = configuration.GetSection(config);
        // get the value of every key in the config from the key vault and set it to the config
        foreach (var item in section.GetChildren())
        {
            try
            {
                var key = item.Key;
                var value = keyVaultService.Get(key).Result;
                if (!string.IsNullOrEmpty(value))
                {
                    section[key] = value;
                }
            }
            catch (System.Exception)
            {
                continue;
            }
        }
        services.Configure<T>(section);
        return services;
    }
}

