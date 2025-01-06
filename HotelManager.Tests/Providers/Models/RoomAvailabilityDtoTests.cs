using HotelManager.Providers.Models;

namespace HotelManager.Tests.Providers.Models;

public class RoomAvailabilityDtoTests
{
    [TestCaseSource(nameof(RoomAvailabilityCases))]
    public void ToFormattedString_Should_Return_Correct_Format(RoomAvailabilityDto roomAvailability, string expectedResult)
    {
        var formattedString = roomAvailability.ToFormattedString();
        
        Assert.That(formattedString, Is.EqualTo(expectedResult));
    }
    
    public static object[] RoomAvailabilityCases =
    [
        new object[] 
        { 
            new RoomAvailabilityDto 
            { 
                StartDate = new DateOnly(2024, 12, 25),
                EndDate = new DateOnly(2025, 11, 14),
                NumberOfRooms = 4
            }, 
            "(20241225-20251114, 4)"
        },
        new object[] 
        { 
            new RoomAvailabilityDto
            {
                StartDate = new DateOnly(1993, 1, 2),
                EndDate = new DateOnly(1993, 1, 2),
                NumberOfRooms = 7
            },
            "(19930102, 7)"
        }
    ];
}