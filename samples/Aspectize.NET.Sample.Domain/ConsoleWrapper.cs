namespace Aspectize.NET.Sample.Domain;

public class ConsoleWrapper : IConsoleWrapper
{
    /// <inheritdoc />
    public void Log(string message)
    {
        Console.WriteLine(message);
    }
}