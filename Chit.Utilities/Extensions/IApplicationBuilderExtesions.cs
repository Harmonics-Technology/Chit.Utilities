using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Chit.Utilities;

public static class IApplicationBuilderExtensions
{
    public static void UseQueryableExtensions(this WebApplication app)
    {
        IQueryableExtensions.Configure(app.Services.GetRequiredService<IMapper>(), app.Services.GetRequiredService<AutoMapper.IConfigurationProvider>());
    }
}
