using CSharpFunctionalExtensions;
using HotelManager.Common;
using HotelManager.Providers;
using HotelManager.Read.Entities;
using HotelManager.Read.Repositories;
using Moq;

namespace HotelManager.Tests.Providers;

public class HotelAvailabilityProviderTests
{
    private Mock<IHotelRepository> _mockHotelRepository;
    private Mock<IBookingRepository> _mockBookingRepository;
    private Mock<ISystemTimeProvider> _mockSystemTimeProvider;
    private HotelAvailabilityProvider _hotelAvailabilityProvider;

    [SetUp]
    public void Setup()
    {
        _mockHotelRepository = new Mock<IHotelRepository>();
        _mockBookingRepository = new Mock<IBookingRepository>();
        _mockSystemTimeProvider = new Mock<ISystemTimeProvider>();

        _hotelAvailabilityProvider = new HotelAvailabilityProvider(
            _mockHotelRepository.Object,
            _mockBookingRepository.Object,
            _mockSystemTimeProvider.Object
        );
    }

    [Test]
    public void GetRoomAvailabilities_Should_Return_Success_When_Rooms_Are_Available()
    {
        // Arrange
        const string hotelId = "H1";
        const string roomTypeCode = "SGL";
        const int numberOfDaysToSearch = 5;
        var currentDate = new DateOnly(2024, 9, 1);

        var hotel = new Hotel
        {
            Id = hotelId,
            Name = "Hotel",
            RoomTypes = [],
            Rooms = new List<Room>
            {
                new() { RoomId = "101", RoomType = roomTypeCode },
                new() { RoomId = "102", RoomType = roomTypeCode }
            }
        };

        var bookings = new List<Booking>
        {
            new()
            {
                HotelId = hotelId,
                RoomType = roomTypeCode,
                Arrival = new DateOnly(2024, 9, 2),
                Departure = new DateOnly(2024, 9, 3),
                RoomRate = "Standard"
            }
        };

        _mockHotelRepository.Setup(r => r.GetHotels()).Returns(Result.Success(new List<Hotel> { hotel }.AsEnumerable()));
        _mockBookingRepository.Setup(r => r.GetBookings()).Returns(Result.Success(bookings.AsEnumerable()));
        _mockSystemTimeProvider.Setup(st => st.GetCurrentDateUtc()).Returns(currentDate);

        // Act
        var result = _hotelAvailabilityProvider.GetRoomAvailabilities(hotelId, numberOfDaysToSearch, roomTypeCode);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(result.Value.First().StartDate, Is.EqualTo(new DateOnly(2024, 9, 2)));
            Assert.That(result.Value.First().EndDate, Is.EqualTo(new DateOnly(2024, 9, 2)));
            Assert.That(result.Value.First().NumberOfRooms, Is.EqualTo(1));
            
            Assert.That(result.Value.Last().StartDate, Is.EqualTo(new DateOnly(2024, 9, 3)));
            Assert.That(result.Value.Last().EndDate, Is.EqualTo(new DateOnly(2024, 9, 6)));
            Assert.That(result.Value.Last().NumberOfRooms, Is.EqualTo(2));
        });
    }

    [Test]
    public void GetRoomAvailabilities_Should_Return_Failure_When_Hotel_Not_Found()
    {
        // Arrange
        const string hotelId = "InvalidHotelId";
        const string roomTypeCode = "SGL";
        const int numberOfDaysToSearch = 5;

        _mockHotelRepository.Setup(r => r.GetHotels()).Returns(Result.Success(new List<Hotel>().AsEnumerable()));
        _mockBookingRepository.Setup(r => r.GetBookings()).Returns(Result.Success(new List<Booking>().AsEnumerable()));

        // Act
        var result = _hotelAvailabilityProvider.GetRoomAvailabilities(hotelId, numberOfDaysToSearch, roomTypeCode);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.EqualTo($"Hotel with id: {hotelId} does not exist"));
    }

    [Test]
    public void GetNumberOfAvailableRooms_Should_Return_Success_When_Rooms_Are_Available()
    {
        // Arrange
        const string hotelId = "H1";
        const string roomTypeCode = "SGL";
        var startDate = new DateOnly(2024, 9, 1);
        var endDate = new DateOnly(2024, 9, 5);

        var hotel = new Hotel
        {
            Id = hotelId,
            Name = "Hotel",
            RoomTypes = [],
            Rooms = new List<Room>
            {
                new() { RoomId = "101", RoomType = roomTypeCode },
                new() { RoomId = "102", RoomType = roomTypeCode }
            }
        };

        var bookings = new List<Booking>
        {
            new()
            {
                HotelId = hotelId,
                RoomType = roomTypeCode,
                Arrival = new DateOnly(2024, 9, 2),
                Departure = new DateOnly(2024, 9, 3),
                RoomRate = "Standard"
            }
        };

        _mockHotelRepository.Setup(r => r.GetHotels()).Returns(Result.Success(new List<Hotel> { hotel }.AsEnumerable()));
        _mockBookingRepository.Setup(r => r.GetBookings()).Returns(Result.Success(bookings.AsEnumerable()));

        // Act
        var result = _hotelAvailabilityProvider.GetNumberOfAvailableRooms(hotelId, roomTypeCode, startDate, endDate);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(1));
    }

    [Test]
    public void GetNumberOfAvailableRooms_Should_Return_Failure_When_Hotel_Not_Found()
    {
        // Arrange
        const string hotelId = "InvalidHotelId";
        const string roomTypeCode = "SGL";
        var startDate = new DateOnly(2024, 9, 1);
        var endDate = new DateOnly(2024, 9, 5);

        _mockHotelRepository.Setup(r => r.GetHotels()).Returns(Result.Success(new List<Hotel>().AsEnumerable()));
        _mockBookingRepository.Setup(r => r.GetBookings()).Returns(Result.Success(new List<Booking>().AsEnumerable()));

        // Act
        var result = _hotelAvailabilityProvider.GetNumberOfAvailableRooms(hotelId, roomTypeCode, startDate, endDate);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.EqualTo($"Hotel with id: {hotelId} does not exist"));
    }

    [Test]
    public void GetNumberOfAvailableRooms_Should_Return_Zero_When_No_Rooms_Are_Available()
    {
        // Arrange
        const string hotelId = "H1";
        const string roomTypeCode = "SGL";
        var startDate = new DateOnly(2024, 9, 1);
        var endDate = new DateOnly(2024, 9, 5);

        var hotel = new Hotel
        {
            Id = hotelId,
            Name = "Hotel",
            RoomTypes = [],
            Rooms = new List<Room>
            {
                new() { RoomId = "101", RoomType = roomTypeCode },
                new() { RoomId = "102", RoomType = roomTypeCode }
            }
        };

        var bookings = new List<Booking>
        {
            new()
            {
                HotelId = hotelId,
                RoomType = roomTypeCode,
                Arrival = new DateOnly(2024, 9, 1),
                Departure = new DateOnly(2024, 9, 3),
                RoomRate = "Standard"
            },
            new()
            {
                HotelId = hotelId,
                RoomType = roomTypeCode,
                Arrival = new DateOnly(2024, 9, 1),
                Departure = new DateOnly(2024, 9, 3),
                RoomRate = "Standard"
            }
        };

        _mockHotelRepository.Setup(r => r.GetHotels()).Returns(Result.Success(new List<Hotel> { hotel }.AsEnumerable()));
        _mockBookingRepository.Setup(r => r.GetBookings()).Returns(Result.Success(bookings.AsEnumerable()));

        // Act
        var result = _hotelAvailabilityProvider.GetNumberOfAvailableRooms(hotelId, roomTypeCode, startDate, endDate);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(0));
    }
}