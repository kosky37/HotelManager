using HotelManager.Commands;
using HotelManager.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManager;

public static class Registry
{
    public static IServiceCollection AddInterface(this IServiceCollection services)
    {
        services.AddTransient<IInputLoop, InputLoop>();
        services.AddTransient<ICommandExecutor, CommandExecutor>();
        services.AddTransient<IHotelAvailabilityProvider, HotelAvailabilityProvider>();
        services.AddConsoleCommands();
        
        return services;
    }
    
    private static void AddConsoleCommands(this IServiceCollection services)
    {
        services.AddTransient<IConsoleCommand, AvailabilityCommand>();
        services.AddTransient<IConsoleCommand, SearchCommand>();
    }
}