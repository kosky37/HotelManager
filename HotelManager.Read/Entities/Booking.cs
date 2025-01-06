namespace HotelManager.Read.Entities;

public class Booking
{
    public required string HotelId { get; init; }
    public required DateOnly Arrival { get; init; }
    public required DateOnly Departure { get; init; }
    public required string RoomType { get; init; }
    public required string RoomRate { get; init; }
}