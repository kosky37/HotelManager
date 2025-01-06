using HotelManager.Common;

namespace HotelManager.Providers.Models;

public class RoomAvailabilityDto
{
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
    public required int NumberOfRooms { get; init; }

    public string ToFormattedString()
    {
        return StartDate == EndDate
            ? $"({StartDate.ToString(Consts.DateFormat)}, {NumberOfRooms})"
            : $"({StartDate.ToString(Consts.DateFormat)}-{EndDate.ToString(Consts.DateFormat)}, {NumberOfRooms})";
    }
}