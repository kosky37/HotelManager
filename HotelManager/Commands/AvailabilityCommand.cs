using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using HotelManager.Common;
using HotelManager.Providers;

namespace HotelManager.Commands;

public partial class AvailabilityCommand : IConsoleCommand
{
    private readonly IHotelAvailabilityProvider _hotelAvailabilityProvider;
    
    private const string Pattern = @"Availability\((\w+),\s*(\d{8})(?:-(\d{8}))?,\s*(\w{3})\)";

    public AvailabilityCommand(IHotelAvailabilityProvider hotelAvailabilityProvider)
    {
        _hotelAvailabilityProvider = hotelAvailabilityProvider;
    }
    
    public bool CanHandle(string inputString)
    {
        return CommandSyntaxRegex().IsMatch(inputString);
    }

    public Result<string> Handle(string inputString)
    {
        if (!TryParse(inputString, out var hotelId, out var roomTypeCode, out var startDate, out var endDate))
        {
            return Result.Failure<string>("Invalid input");
        }

        return _hotelAvailabilityProvider.GetNumberOfAvailableRooms(hotelId, roomTypeCode, startDate, endDate)
            .Map(numberOfAvailableRooms => numberOfAvailableRooms.ToString());
    }

    private static bool TryParse(
        string inputString, 
        out string hotelId,
        out string roomTypeCode,
        out DateOnly startDate,
        out DateOnly endDate)
    {
        var match = CommandSyntaxRegex().Match(inputString);

        var parsedSuccessfully = true;
        
        hotelId = match.Groups[1].Value;
        var unparsedStartDate = match.Groups[2].Value;
        var unparsedEndDate = match.Groups[3].Length > 0 ? match.Groups[3].Value : match.Groups[2].Value;
        roomTypeCode = match.Groups[4].Value;
        
        parsedSuccessfully &= DateOnly.TryParseExact(unparsedStartDate, Consts.DateFormat, out startDate);
        parsedSuccessfully &= DateOnly.TryParseExact(unparsedEndDate, Consts.DateFormat, out endDate);
        return parsedSuccessfully;
    }

    [GeneratedRegex(Pattern)]
    private static partial Regex CommandSyntaxRegex();
}