using CSharpFunctionalExtensions;

namespace HotelManager.Commands;

public interface IConsoleCommand
{
    bool CanHandle(string inputString);
    Result<string> Handle(string inputString);
}