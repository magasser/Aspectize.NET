namespace Aspectize.NET.Sample.Domain;

public sealed class ConsoleLogAspect : Aspect
{
    private readonly IConsoleWrapper _console;

    public ConsoleLogAspect(IConsoleWrapper console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    /// <inheritdoc />
    public override void Before(IInvocationContext context)
    {
        _console.Log($"{nameof(ConsoleLogAspect)} => {context.TargetType.Name}.{context.Method.Name}()");
    }

    /// <inheritdoc />
    public override void After(IInvocationContext context)
    {
        _console.Log(
            $"{nameof(ConsoleLogAspect)} => {context.TargetType.Name}.{context.Method.Name}() returned {context.SynchronousReturnValue ?? (context.AsynchronousReturnValue ?? "void")}");
    }
}