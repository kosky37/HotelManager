using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using HotelManager.Providers;

namespace HotelManager.Commands;

public partial class SearchCommand : IConsoleCommand
{
    private readonly IHotelAvailabilityProvider _hotelAvailabilityProvider;
    
    private const string Pattern = @"Search\((\w+),\s*(\d+),\s*(\w{3})\)";

    public SearchCommand(IHotelAvailabilityProvider hotelAvailabilityProvider)
    {
        _hotelAvailabilityProvider = hotelAvailabilityProvider;
    }

    public bool CanHandle(string inputString)
    {
        return CommandSyntaxRegex().IsMatch(inputString);
    }

    public Result<string> Handle(string inputString)
    {
        if (!TryParse(inputString, out var hotelId, out var roomTypeCode, out var numberOfDays))
        {
            return Result.Failure<string>("Invalid input");
        }

        return _hotelAvailabilityProvider.GetRoomAvailabilities(hotelId, numberOfDays, roomTypeCode)
            .Map(roomAvailabilities =>
                string.Join(", ", roomAvailabilities.Select(x => x.ToFormattedString())
                ));
    }
    
    private static bool TryParse(string inputString, out string hotelId, out string roomTypeCode,
        out int numberOfDays)
    {
        var match = CommandSyntaxRegex().Match(inputString);

        var parsedSuccessfully = true;
        
        hotelId = match.Groups[1].Value;
        var unparsedNumberOfDays = match.Groups[2].Value;
        roomTypeCode = match.Groups[3].Value;
        
        parsedSuccessfully &= int.TryParse(unparsedNumberOfDays, out numberOfDays);
        return parsedSuccessfully;
    }

    [GeneratedRegex(Pattern)]
    private static partial Regex CommandSyntaxRegex();
}