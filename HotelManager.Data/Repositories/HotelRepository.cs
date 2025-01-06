using System.Text.Json;
using CSharpFunctionalExtensions;
using HotelManager.Common;
using HotelManager.Read.Entities;
using HotelManager.Read.Repositories;

namespace HotelManager.Data.Repositories;

internal class HotelRepository : IHotelRepository
{
    private readonly IFileInfo _hotelsFileInfo;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public HotelRepository(IFileInfo hotelsFileInfo, JsonSerializerOptions jsonSerializerOptions)
    {
        _hotelsFileInfo = hotelsFileInfo;
        _jsonSerializerOptions = jsonSerializerOptions;
    }
    
    public Result<IEnumerable<Hotel>> GetHotels()
    {
        return Result.SuccessIf(_hotelsFileInfo.Exists, "Hotels file not found.")
            .Bind(() => DeserializeJsonFile(_hotelsFileInfo));
    }
    
    private Result<IEnumerable<Hotel>> DeserializeJsonFile(IFileInfo fileInfo)
    {
        try
        {
            using var stream = fileInfo.OpenRead();
            return Result.Success(JsonSerializer.Deserialize<IEnumerable<Hotel>>(stream, _jsonSerializerOptions)!);
        }
        catch (Exception)
        {
            return Result.Failure<IEnumerable<Hotel>>("Unable to deserialize hotels file.");
        }
    }
}