using CSharpFunctionalExtensions;
using HotelManager.Commands;
using HotelManager.Providers;
using Moq;

namespace HotelManager.Tests.Commands;

public class AvailabilityCommandTests
{
    private Mock<IHotelAvailabilityProvider> _mockHotelAvailabilityProvider;
    private AvailabilityCommand _availabilityCommand;

    [SetUp]
    public void Setup()
    {
        _mockHotelAvailabilityProvider = new Mock<IHotelAvailabilityProvider>();
        _availabilityCommand = new AvailabilityCommand(_mockHotelAvailabilityProvider.Object);
    }

    [TestCase("Availability(H1, 20240901-20240903, SGL)")]
    [TestCase("Availability(H1, 20240901, SGL)")]
    public void CanHandle_Should_Return_True_When_Input_Matches_Pattern(string validInput)
    {
        // Act
        var result = _availabilityCommand.CanHandle(validInput);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void CanHandle_Should_Return_False_When_Input_Does_Not_Match_Pattern()
    {
        // Arrange
        const string invalidInput = "InvalidInput";

        // Act
        var result = _availabilityCommand.CanHandle(invalidInput);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Handle_Should_Return_Success_When_Input_Is_Valid()
    {
        // Arrange
        const string validInput = "Availability(H1, 20240901-20240903, SGL)";
        const string hotelId = "H1";
        const string roomTypeCode = "SGL";
        var startDate = new DateOnly(2024, 9, 1);
        var endDate = new DateOnly(2024, 9, 3);
        const int availableRooms = 5;

        _mockHotelAvailabilityProvider
            .Setup(p => p.GetNumberOfAvailableRooms(hotelId, roomTypeCode, startDate, endDate))
            .Returns(Result.Success(availableRooms));

        // Act
        var result = _availabilityCommand.Handle(validInput);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(availableRooms.ToString()));
    }

    [Test]
    public void Handle_Should_Return_Failure_When_Cannot_Parse_Input()
    {
        // Arrange
        const string invalidInput = "Availability(H1, 20240901, invalid)";

        // Act
        var result = _availabilityCommand.Handle(invalidInput);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.EqualTo("Invalid input"));
    }
}