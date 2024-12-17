namespace Aspectize.NET.Sample.Domain;

[Aspect<ConsoleLogAspect>]
public interface ISampleInterface
{
    void Call();

    int CallWithReturn();

    Task CallAsync(CancellationToken cancellationToken = default);

    Task<int> CallWithReturnAsync(CancellationToken cancellationToken = default);
}

public interface ISampleInterface2 : ISampleInterface
{
    void Method();
}