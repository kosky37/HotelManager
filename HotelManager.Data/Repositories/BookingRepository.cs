using System.Text.Json;
using CSharpFunctionalExtensions;
using HotelManager.Common;
using HotelManager.Read.Entities;
using HotelManager.Read.Repositories;

namespace HotelManager.Data.Repositories;

internal class BookingRepository : IBookingRepository
{
    private readonly IFileInfo _bookingsFileInfo;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public BookingRepository(IFileInfo bookingsFileInfo, JsonSerializerOptions jsonSerializerOptions)
    {
        _bookingsFileInfo = bookingsFileInfo;
        _jsonSerializerOptions = jsonSerializerOptions;
    }
    
    public Result<IEnumerable<Booking>> GetBookings()
    {
        return Result.SuccessIf(_bookingsFileInfo.Exists, "Bookings file not found.")
            .Bind(() => DeserializeJsonFile(_bookingsFileInfo));
    }
    
    private Result<IEnumerable<Booking>> DeserializeJsonFile(IFileInfo fileInfo)
    {
        try
        {
            using var stream = fileInfo.OpenRead();
            return Result.Success(JsonSerializer.Deserialize<IEnumerable<Booking>>(stream, _jsonSerializerOptions)!);
        }
        catch (Exception)
        {
            return Result.Failure<IEnumerable<Booking>>("Unable to deserialize bookings file.");
        }
    }
}