namespace HotelManager.Read.Entities;

public class Hotel
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required IEnumerable<RoomType> RoomTypes { get; init; }
    public required IEnumerable<Room> Rooms { get; init; }
}