namespace HotelManager.Common;

public interface IConsole
{
    string? Read();
    void WriteError(string message);
    void WriteLine(string message);
}

public class Console : IConsole
{
    public string? Read()
    {
        return System.Console.ReadLine();
    }

    public void WriteError(string message)
    {
        System.Console.WriteLine(message, ConsoleColor.Red);
    }

    public void WriteLine(string message)
    {
        System.Console.WriteLine(message);
    }
}