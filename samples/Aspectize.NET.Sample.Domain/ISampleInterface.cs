namespace Aspectize.NET.Sample.Domain;

public interface ISampleInterface
{
    [Aspect<ConsoleLogAspect>]
    void Call();

    Task CallAsync(CancellationToken cancellationToken = default);

    Task<int> CallWithReturnAsync(CancellationToken cancellationToken = default);
}

public interface ISampleInterface2 : ISampleInterface
{
    void Method();
}