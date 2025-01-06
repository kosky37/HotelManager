using CSharpFunctionalExtensions;
using HotelManager.Common;

namespace HotelManager;

public interface IInputLoop
{
    void Run();
}

internal class InputLoop : IInputLoop
{
    private readonly IConsole _console;
    private readonly ICommandExecutor _commandExecutor;

    public InputLoop(IConsole console, ICommandExecutor commandExecutor)
    {
        _console = console;
        _commandExecutor = commandExecutor;
    }
    
    public void Run()
    {
        while (AwaitCommand()) { }
    }

    private bool AwaitCommand()
    {
        var inputString = _console.Read();

        if (inputString == "Exit") return false;
        if (string.IsNullOrWhiteSpace(inputString)) return true;

        _commandExecutor.Execute(inputString).Match(_console.WriteLine, _console.WriteError);

        return true;
    }
}