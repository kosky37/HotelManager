using CSharpFunctionalExtensions;
using HotelManager.Read.Entities;

namespace HotelManager.Read.Repositories;

public interface IHotelRepository
{
    Result<IEnumerable<Hotel>> GetHotels();
}