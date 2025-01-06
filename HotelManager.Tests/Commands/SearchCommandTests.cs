using HotelManager.Commands;
using HotelManager.Providers;
using HotelManager.Providers.Models;
using Moq;
using CSharpFunctionalExtensions;

namespace HotelManager.Tests.Commands;

public class SearchCommandTests
{
    private Mock<IHotelAvailabilityProvider> _mockHotelAvailabilityProvider;
    private SearchCommand _searchCommand;

    [SetUp]
    public void Setup()
    {
        _mockHotelAvailabilityProvider = new Mock<IHotelAvailabilityProvider>();
        _searchCommand = new SearchCommand(_mockHotelAvailabilityProvider.Object);
    }

    [Test]
    public void CanHandle_Should_Return_True_When_Input_Matches_Pattern()
    {
        // Arrange
        const string validInput = "Search(H1, 3, SGL)";

        // Act
        var result = _searchCommand.CanHandle(validInput);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void CanHandle_Should_Return_False_When_Input_Does_Not_Match_Pattern()
    {
        // Arrange
        const string invalidInput = "InvalidInput";

        // Act
        var result = _searchCommand.CanHandle(invalidInput);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Handle_Should_Return_Success_When_Input_Is_Valid()
    {
        // Arrange
        const string validInput = "Search(H1, 3, SGL)";
        const string hotelId = "H1";
        const string roomTypeCode = "SGL";
        const int numberOfDays = 3;

        var roomAvailabilities = new List<RoomAvailabilityDto>
        {
            new() { StartDate = new DateOnly(2024, 9, 1), EndDate = new DateOnly(2024, 9, 3), NumberOfRooms = 1 },
            new() { StartDate = new DateOnly(2024, 9, 2), EndDate = new DateOnly(2024, 9, 5), NumberOfRooms = 2 }
        };

        _mockHotelAvailabilityProvider.Setup(p => p.GetRoomAvailabilities(hotelId, numberOfDays, roomTypeCode))
            .Returns(Result.Success(roomAvailabilities));

        // Act
        var result = _searchCommand.Handle(validInput);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("(20240901-20240903, 1), (20240902-20240905, 2)"));
    }

    [Test]
    public void Handle_Should_Return_Failure_When_Cannot_Parse_Input()
    {
        // Arrange
        const string invalidInput = "Search(H1, invalid, SGL)";

        // Act
        var result = _searchCommand.Handle(invalidInput);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.EqualTo("Invalid input"));
    }
}