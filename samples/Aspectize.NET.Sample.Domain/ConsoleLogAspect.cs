namespace Aspectize.NET.Sample.Domain;

public sealed class ConsoleLogAspect : IBeforeAspect, IAfterAspect
{
    private readonly IConsoleWrapper _console;

    public ConsoleLogAspect(IConsoleWrapper console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    /// <inheritdoc />
    public void After(IAfterInvocationContext context)
    {
        _console.Log($"{nameof(ConsoleLogAspect)} => {context.TargetType.Name}.{context.Method.Name}()");
    }

    /// <inheritdoc />
    public void Before(IBeforeInvocationContext context)
    {
        _console.Log($"{nameof(ConsoleLogAspect)} => {context.TargetType.Name}.{context.Method.Name}()");
    }
}