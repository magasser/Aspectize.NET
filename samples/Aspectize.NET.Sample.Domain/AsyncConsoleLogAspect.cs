namespace Aspectize.NET.Sample.Domain;

public sealed class AsyncConsoleLogAspect : Aspect, IAsyncBeforeAspect, IAsyncAfterAspect
{
    private readonly IConsoleWrapper _console;

    public AsyncConsoleLogAspect(IConsoleWrapper console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    /// <inheritdoc />
    public Task AfterAsync(IAfterInvocationContext context)
    {
        _console.Log($"{nameof(AsyncConsoleLogAspect)} => {context.TargetType.Name}.{context.Method.Name}()");

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task BeforeAsync(IBeforeInvocationContext context)
    {
        _console.Log($"{nameof(AsyncConsoleLogAspect)} => {context.TargetType.Name}.{context.Method.Name}()");

        return Task.CompletedTask;
    }
}