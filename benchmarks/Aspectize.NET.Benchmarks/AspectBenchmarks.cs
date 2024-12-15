using BenchmarkDotNet.Attributes;

namespace Aspectize.NET.Benchmarks;

public class AspectBenchmarks
{
    private readonly IBenchmarkAspectInterface _original;
    private readonly IBenchmarkAspectInterface _aspectized;

    public AspectBenchmarks()
    {
        var configuration = AspectConfigurationBuilder.Create()
                                                      .Use(new BenchmarkAspect())
                                                      .Use(new AsyncBenchmarkAspect())
                                                      .Build();

        var binder = new AspectBinder(configuration);

        _original = new BenchmarkAspectImpl();
        _aspectized = binder.Bind<IBenchmarkAspectInterface>(_original);
    }

    [Benchmark]
    public void OriginalRun()
    {
        _original.Run();
    }

    [Benchmark]
    public void AspectizedRun()
    {
        _aspectized.Run();
    }

    [Benchmark]
    public void OriginalAsyncRun()
    {
        _original.AsyncRun();
    }

    [Benchmark]
    public void AspectizedAsyncRun()
    {
        _aspectized.AsyncRun();
    }

    public class BenchmarkAspect : Aspect, IBeforeAspect, IAfterAspect
    {
        /// <inheritdoc />
        public void After(IAfterInvocationContext context)
        {
            // Do nothing
        }

        /// <inheritdoc />
        public void Before(IBeforeInvocationContext context)
        {
            // Do nothing
        }
    }

    public class AsyncBenchmarkAspect : Aspect, IAsyncBeforeAspect, IAsyncAfterAspect
    {
        /// <inheritdoc />
        public Task AfterAsync(IAfterInvocationContext context)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task BeforeAsync(IBeforeInvocationContext context)
        {
            return Task.CompletedTask;
        }
    }

    public interface IBenchmarkAspectInterface
    {
        [Aspect<BenchmarkAspect>]
        void Run();

        [Aspect<AsyncBenchmarkAspect>]
        void AsyncRun();
    }

    public class BenchmarkAspectImpl : IBenchmarkAspectInterface
    {
        public void Run()
        {
            // Do nothing
        }

        public void AsyncRun()
        {
            // Do nothing
        }
    }
}