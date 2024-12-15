namespace Aspectize.NET.Sample.Domain;

public interface ISampleInterface
{
    [Aspect<ConsoleLogAspect>]
    void Call();

    [Aspect<AsyncConsoleLogAspect>]
    Task CallAsync(CancellationToken cancellationToken = default);

    Task<int> CallWithReturnAsync(CancellationToken cancellationToken = default);
}