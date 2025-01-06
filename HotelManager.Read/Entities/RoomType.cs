namespace HotelManager.Read.Entities;

public class RoomType
{
    public required string Code { get; init; }
    public required string Description { get; init; }
    public required IEnumerable<string> Amenities { get; init; }
    public required IEnumerable<string> Features { get; init; }
}