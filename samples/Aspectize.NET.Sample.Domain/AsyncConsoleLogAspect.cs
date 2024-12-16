namespace Aspectize.NET.Sample.Domain;

public sealed class AsyncConsoleLogAspect : Aspect
{
    private readonly IConsoleWrapper _console;

    public AsyncConsoleLogAspect(IConsoleWrapper console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    /// <inheritdoc />
    public override Task BeforeAsync(IInvocationContext context)
    {
        _console.Log($"{nameof(AsyncConsoleLogAspect)} => {context.TargetType.Name}.{context.Method.Name}()");

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task AfterAsync(IInvocationContext context)
    {
        _console.Log($"{nameof(AsyncConsoleLogAspect)} => {context.TargetType.Name}.{context.Method.Name}()");

        return Task.CompletedTask;
    }
}