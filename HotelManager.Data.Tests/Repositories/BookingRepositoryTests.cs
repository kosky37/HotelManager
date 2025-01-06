using System.Text;
using System.Text.Json;
using HotelManager.Common;
using HotelManager.Data.JsonConverters;
using HotelManager.Data.Repositories;
using Moq;

namespace HotelManager.Data.Tests.Repositories;

public class BookingRepositoryTests
{
    [Test]
    public void GetBookings_Should_Fail_If_File_Is_Not_Found()
    {
        // Arrange
        var fileInfoMock = new Mock<IFileInfo>();
        fileInfoMock.Setup(x => x.Exists).Returns(false);
        
        var repository = new BookingRepository(fileInfoMock.Object, new JsonSerializerOptions());
        
        // Act
        var result = repository.GetBookings();
        
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Bookings file not found."));
        });
    }
    
    [Test]
    public void GetBookings_Should_Succeed()
    {
        // Arrange
        const string bookingsJson = """
                                    [
                                        {
                                            "hotelId": "H1",
                                            "arrival": "20240901",
                                            "departure": "20240903",
                                            "roomType": "DBL",
                                            "roomRate": "Prepaid"
                                        },
                                        {
                                            "hotelId": "H1",
                                            "arrival": "20240902",
                                            "departure": "20240905",
                                            "roomType": "SGL",
                                            "roomRate": "Standard"
                                        }
                                    ]
                                    """;
        
        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(bookingsJson));
        var fileInfoMock = new Mock<IFileInfo>();
        
        fileInfoMock.Setup(x => x.Exists).Returns(true);
        fileInfoMock.Setup(x => x.OpenRead()).Returns(memoryStream);
        
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        serializerOptions.Converters.Add(new DateOnlyConverter());
        
        var repository = new BookingRepository(fileInfoMock.Object, serializerOptions);
        
        // Act
        var result = repository.GetBookings();
        
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Count(), Is.EqualTo(2));
            Assert.That(result.Value.First().Arrival, Is.EqualTo(new DateOnly(2024, 9, 1)));
            Assert.That(result.Value.First().Departure, Is.EqualTo(new DateOnly(2024, 9, 3)));
            Assert.That(result.Value.First().RoomType, Is.EqualTo("DBL"));
        });
    }
}