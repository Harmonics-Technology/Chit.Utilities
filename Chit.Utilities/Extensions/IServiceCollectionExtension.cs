using System;
using Microsoft.Extensions.DependencyInjection;

namespace Chit.Utilities;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddUtilities(this IServiceCollection services)
    {
        
        services.AddMvc(options =>
        {
            options.Filters.Add<LinkRewritingFilter>();
        });
        services.AddTransient<IEmailHandler, EmailHandler>();
        return services;
    }
}
