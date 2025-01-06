using System.CommandLine;
using System.CommandLine.Parsing;
using HotelManager;
using HotelManager.Common;
using HotelManager.Data;
using Microsoft.Extensions.DependencyInjection;

var hotelsFileOption = new Option<FileInfo>(
    name: "--hotels",
    description: "The JSON file containing hotels data") { IsRequired = true };

var bookingsFileOption = new Option<FileInfo>(
    name: "--bookings",
    description: "The JSON file containing booking data") { IsRequired = true };

hotelsFileOption.AddValidator(result => ValidateFileExistence(result, hotelsFileOption));
bookingsFileOption.AddValidator(result => ValidateFileExistence(result, bookingsFileOption));

var rootCommand = new RootCommand("Hotel Manager app")
{
    hotelsFileOption,
    bookingsFileOption
};

rootCommand.SetHandler((hotelsFileInfo, bookingsFileInfo) =>
    {
        var serviceProvider = new ServiceCollection()
            .AddCommon()
            .AddData(new MyFileInfo(hotelsFileInfo), new MyFileInfo(bookingsFileInfo))
            .AddInterface()
            .BuildServiceProvider();
        
        var inputLoop = serviceProvider.GetRequiredService<IInputLoop>();
        
        inputLoop.Run();
    },
    hotelsFileOption,
    bookingsFileOption);

return await rootCommand.InvokeAsync(args);


void ValidateFileExistence(OptionResult result, Option<FileInfo> fileInfoOption)
{
    var fileInfo = result.GetValueForOption(fileInfoOption);
    if (!fileInfo!.Exists)
    {
        result.ErrorMessage = $"""File "{fileInfo.FullName}" does not exist.""";
    }
}