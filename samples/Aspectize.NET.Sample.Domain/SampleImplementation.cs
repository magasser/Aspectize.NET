namespace Aspectize.NET.Sample.Domain;

public class SampleImplementation : ISampleInterface2
{
    /// <inheritdoc />
    public void Call() { }

    /// <inheritdoc />
    public int CallWithReturn()
    {
        return 41;
    }

    /// <inheritdoc />
    public Task CallAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<int> CallWithReturnAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(42);
    }

    /// <inheritdoc />
    public void Method() { }
}