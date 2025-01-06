using System.Text;
using System.Text.Json;
using HotelManager.Common;
using HotelManager.Data.Repositories;
using Moq;

namespace HotelManager.Data.Tests.Repositories;

public class HotelRepositoryTests
{
    [Test]
    public void GetHotels_Should_Fail_If_File_Is_Not_Found()
    {
        // Arrange
        var fileInfoMock = new Mock<IFileInfo>();
        fileInfoMock.Setup(x => x.Exists).Returns(false);
        
        var repository = new HotelRepository(fileInfoMock.Object, new JsonSerializerOptions());
        
        // Act
        var result = repository.GetHotels();
        
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Hotels file not found."));
        });
    }
    
    [Test]
    public void GetHotels_Should_Succeed()
    {
        // Arrange
        const string hotelsJson = """
                                  [
                                      {
                                          "id": "H1",
                                          "name": "Hotel California",
                                          "roomTypes": [
                                              {
                                                  "code": "SGL",
                                                  "description": "Single Room",
                                                  "amenities": ["WiFi", "TV"],
                                                  "features": ["Non-smoking"]
                                              },
                                              {
                                                  "code": "DBL",
                                                  "description": "Double Room",
                                                  "amenities": ["WiFi", "TV", "Minibar"],
                                                  "features": ["Non-smoking", "Sea View"]
                                              }
                                          ],
                                          "rooms": [
                                              {
                                                  "roomType": "SGL",
                                                  "roomId": "101"
                                              },
                                              {
                                                  "roomType": "SGL",
                                                  "roomId": "102"
                                              },
                                              {
                                                  "roomType": "DBL",
                                                  "roomId": "201"
                                              },
                                              {
                                                  "roomType": "DBL",
                                                  "roomId": "202"
                                              }
                                          ]
                                      }
                                  ]
                                  """;
        
        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(hotelsJson));
        var fileInfoMock = new Mock<IFileInfo>();
        
        fileInfoMock.Setup(x => x.Exists).Returns(true);
        fileInfoMock.Setup(x => x.OpenRead()).Returns(memoryStream);
        
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        var repository = new HotelRepository(fileInfoMock.Object, serializerOptions);
        
        // Act
        var result = repository.GetHotels();
        
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Count(), Is.EqualTo(1));
            Assert.That(result.Value.Single().Id, Is.EqualTo("H1"));
            Assert.That(result.Value.Single().Name, Is.EqualTo("Hotel California"));
            Assert.That(result.Value.Single().Rooms.Count(), Is.EqualTo(4));
            Assert.That(result.Value.Single().RoomTypes.Count(), Is.EqualTo(2));
        });
    }
}