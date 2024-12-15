namespace Aspectize.NET.Sample.Domain;

public class SampleImplementation : ISampleInterface
{
    /// <inheritdoc />
    public void Call()
    {
        Console.WriteLine("Call");
    }

    /// <inheritdoc />
    public async Task CallAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("CallAsync before delay");

        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);

        Console.WriteLine("CallAsync after delay");
    }

    /// <inheritdoc />
    public async Task<int> CallWithReturnAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("CallWithReturnAsync before delay");

        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);

        Console.WriteLine("CallWithReturnAsync after delay");

        return 42;
    }
}