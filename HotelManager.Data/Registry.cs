using System.Text.Json;
using HotelManager.Common;
using HotelManager.Data.JsonConverters;
using HotelManager.Data.Repositories;
using HotelManager.Read.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManager.Data;

public static class Registry
{
    public static IServiceCollection AddData(this IServiceCollection services, IFileInfo hotelsFileInfo, IFileInfo bookingsFileInfo)
    {
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        serializerOptions.Converters.Add(new DateOnlyConverter());
        
        services.AddTransient<IHotelRepository>(_ => new HotelRepository(hotelsFileInfo, serializerOptions));
        services.AddTransient<IBookingRepository>(_ => new BookingRepository(bookingsFileInfo, serializerOptions));
        
        return services;
    }
}