using CSharpFunctionalExtensions;
using HotelManager.Common;
using HotelManager.Providers.Models;
using HotelManager.Read.Repositories;

namespace HotelManager.Providers;

public interface IHotelAvailabilityProvider
{
    Result<List<RoomAvailabilityDto>> GetRoomAvailabilities(string hotelId, int numberOfDaysToSearch, string roomTypeCode);
    Result<int> GetNumberOfAvailableRooms(
        string hotelId,
        string roomTypeCode,
        DateOnly startDate,
        DateOnly endDate);
}

internal class HotelAvailabilityProvider : IHotelAvailabilityProvider
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly ISystemTimeProvider _systemTimeProvider;

    public HotelAvailabilityProvider(IHotelRepository hotelRepository, IBookingRepository bookingRepository, ISystemTimeProvider systemTimeProvider)
    {
        _hotelRepository = hotelRepository;
        _bookingRepository = bookingRepository;
        _systemTimeProvider = systemTimeProvider;
    }

    public Result<List<RoomAvailabilityDto>> GetRoomAvailabilities(string hotelId, int numberOfDaysToSearch, string roomTypeCode)
    {
        var currentDate = _systemTimeProvider.GetCurrentDateUtc();
        var earliestIncludedDate = currentDate.AddDays(1);
        var lastIncludedDate = currentDate.AddDays(numberOfDaysToSearch);

        var hotelsResult = _hotelRepository.GetHotels();
        var bookingsResult = _bookingRepository.GetBookings();

        return Result.Combine(hotelsResult, bookingsResult)
            .Bind(() =>
            {
                var hotel = hotelsResult.Value.SingleOrDefault(hotel => hotel.Id == hotelId);
                if (hotel == null)
                {
                    return Result.Failure<List<RoomAvailabilityDto>>($"Hotel with id: {hotelId} does not exist");
                }
                
                var rooms = hotel.Rooms.Where(room => room.RoomType == roomTypeCode);
                
                var relevantBookings = bookingsResult.Value.Where(booking => booking.HotelId == hotelId 
                                                                             && booking.RoomType == roomTypeCode
                                                                             && booking.Departure > earliestIncludedDate
                                                                             && booking.Arrival <= lastIncludedDate);

                var dailyAvailabilityTable = new int[numberOfDaysToSearch];
                Array.Fill(dailyAvailabilityTable, rooms.Count());
                foreach (var booking in relevantBookings)
                {
                    var arrivalDayIndex = int.Max(earliestIncludedDate.DayNumber, booking.Arrival.DayNumber) - earliestIncludedDate.DayNumber;
                    var departureDayIndex = int.Min(lastIncludedDate.DayNumber + 1, booking.Departure.DayNumber) - earliestIncludedDate.DayNumber;

                    for (var dayIndex = arrivalDayIndex; dayIndex < departureDayIndex; dayIndex++)
                    {
                        dailyAvailabilityTable[dayIndex] -= 1;
                    }
                }

                var startDateIndex = 0;
                var numberOfRooms = dailyAvailabilityTable[0];
                var availabilities = new List<RoomAvailabilityDto>();
                
                for (var dayIndex = 0; dayIndex < numberOfDaysToSearch; dayIndex++)
                {
                    if (numberOfRooms != dailyAvailabilityTable[dayIndex])
                    {
                        if (numberOfRooms > 0)
                        {
                            availabilities.Add(new RoomAvailabilityDto
                            {
                                NumberOfRooms = numberOfRooms,
                                StartDate = DateOnly.FromDayNumber(earliestIncludedDate.DayNumber + startDateIndex),
                                EndDate = DateOnly.FromDayNumber(earliestIncludedDate.DayNumber + dayIndex - 1)
                            });
                        }

                        startDateIndex = dayIndex;
                        numberOfRooms = dailyAvailabilityTable[dayIndex];
                    }
                }

                if (numberOfRooms > 0)
                {
                    availabilities.Add(new RoomAvailabilityDto
                    {
                        NumberOfRooms = numberOfRooms,
                        StartDate = DateOnly.FromDayNumber(earliestIncludedDate.DayNumber + startDateIndex),
                        EndDate = DateOnly.FromDayNumber(earliestIncludedDate.DayNumber + numberOfDaysToSearch - 1)
                    });
                }

                return Result.Success(availabilities);
            });
    }

    public Result<int> GetNumberOfAvailableRooms(string hotelId, string roomTypeCode, DateOnly startDate, DateOnly endDate)
    {
        var hotelsResult = _hotelRepository.GetHotels();
        var bookingsResult = _bookingRepository.GetBookings();

        return Result.Combine(hotelsResult, bookingsResult)
            .Bind(() =>
            {
                var hotel = hotelsResult.Value.SingleOrDefault(hotel => hotel.Id == hotelId);
                if (hotel == null)
                {
                    return Result.Failure<int>($"Hotel with id: {hotelId} does not exist");
                }

                var rooms = hotel.Rooms.Where(room => room.RoomType == roomTypeCode).ToList();

                if (rooms.Count == 0)
                {
                    return Result.Success(0);
                }

                var relevantBookings = bookingsResult.Value.Where(booking =>
                        booking.HotelId == hotelId
                        && booking.RoomType == roomTypeCode
                        && booking.Departure > startDate
                        && booking.Arrival <= endDate)
                    .ToList();

                if (relevantBookings.Count == 0)
                {
                    return rooms.Count;
                }
                
                var dailyAvailabilityTable = new int[endDate.DayNumber - startDate.DayNumber + 1];
                Array.Fill(dailyAvailabilityTable, rooms.Count);
                foreach (var booking in relevantBookings)
                {
                    var arrivalDayIndex = int.Max(startDate.DayNumber, booking.Arrival.DayNumber) - startDate.DayNumber;
                    var departureDayIndex = int.Min(endDate.DayNumber + 1, booking.Departure.DayNumber) - startDate.DayNumber;

                    for (var dayIndex = arrivalDayIndex; dayIndex < departureDayIndex; dayIndex++)
                    {
                        dailyAvailabilityTable[dayIndex] -= 1;
                    }
                }

                return dailyAvailabilityTable.Min();
            });
    }
}