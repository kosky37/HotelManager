using Microsoft.Extensions.DependencyInjection;

namespace HotelManager.Common;

public static class Registry
{
    public static IServiceCollection AddCommon(this IServiceCollection services)
    {
        services.AddTransient<ISystemTimeProvider, SystemTimeProvider>();
        services.AddTransient<IConsole, Console>();

        return services;
    }
}