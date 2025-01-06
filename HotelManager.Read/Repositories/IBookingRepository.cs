using CSharpFunctionalExtensions;
using HotelManager.Read.Entities;

namespace HotelManager.Read.Repositories;

public interface IBookingRepository
{
    Result<IEnumerable<Booking>> GetBookings();
}