using CSharpFunctionalExtensions;
using HotelManager.Commands;

namespace HotelManager;

public interface ICommandExecutor
{
    Result<string> Execute(string inputString);
}

internal class CommandExecutor : ICommandExecutor
{
    private readonly IEnumerable<IConsoleCommand> _consoleCommands;

    public CommandExecutor(IEnumerable<IConsoleCommand> consoleCommands)
    {
        _consoleCommands = consoleCommands;
    }
    
    public Result<string> Execute(string inputString)
    {
        foreach (var consoleCommand in _consoleCommands)
        {
            if (consoleCommand.CanHandle(inputString))
            {
                return consoleCommand.Handle(inputString);
            }
        }

        return Result.Failure<string>($"Invalid input: {inputString}");
    }
}