using BenchmarkDotNet.Attributes;

using Castle.DynamicProxy;

namespace Aspectize.NET.Benchmarks;

[ShortRunJob]
public class AspectBenchmarks
{
    private readonly IBenchmarkSingleAspect _originalSingle;
    private readonly IBenchmarkDoubleAspect _originalDouble;
    private readonly IBenchmarkSingleAspect _aspectizedSingle;
    private readonly IBenchmarkDoubleAspect _aspectizedDouble;

    public AspectBenchmarks()
    {
        var configuration = AspectConfigurationBuilder.Create()
                                                      .Use(new BenchmarkAspect())
                                                      .Use(new AsyncBenchmarkAspect())
                                                      .Build();

        var binder = new AspectBinder(configuration, new ProxyGenerator());

        _originalSingle = new BenchmarkSingleAspectImpl();
        _originalDouble = new BenchmarkDoubleAspectImpl();
        _aspectizedSingle = binder.Bind<IBenchmarkSingleAspect>(_originalSingle);
        _aspectizedDouble = binder.Bind<IBenchmarkDoubleAspect>(_originalDouble);
    }

    [Benchmark]
    public void OriginalRunSingle()
    {
        _originalSingle.Run();
    }

    [Benchmark]
    public void AspectizedRunSingle()
    {
        _aspectizedSingle.Run();
    }

    [Benchmark]
    public void OriginalAsyncRunSingle()
    {
        _originalSingle.AsyncRun();
    }

    [Benchmark]
    public void AspectizedAsyncRunSingle()
    {
        _aspectizedSingle.AsyncRun();
    }

    [Benchmark]
    public void OriginalRunDouble()
    {
        _originalDouble.Run();
    }

    [Benchmark]
    public void AspectizedRunDouble()
    {
        _aspectizedDouble.Run();
    }

    [Benchmark]
    public void OriginalAsyncRunDouble()
    {
        _originalDouble.AsyncRun();
    }

    [Benchmark]
    public void AspectizedAsyncRunDouble()
    {
        _aspectizedDouble.AsyncRun();
    }

    public class BenchmarkAspect : Aspect
    {
        /// <inheritdoc />
        public override void Before(IInvocationContext context)
        {
            // Do nothing
        }
        
        /// <inheritdoc />
        public override void After(IInvocationContext context)
        {
            // Do nothing
        }
    }

    public class AsyncBenchmarkAspect : Aspect
    {
        /// <inheritdoc />
        public override Task BeforeAsync(IInvocationContext context)
        {
            return Task.CompletedTask;
        }
        
        /// <inheritdoc />
        public override Task AfterAsync(IInvocationContext context)
        {
            return Task.CompletedTask;
        }
    }

    public interface IBenchmarkSingleAspect
    {
        [Aspect<BenchmarkAspect>]
        void Run();

        void AsyncRun();
    }

    public class BenchmarkSingleAspectImpl : IBenchmarkSingleAspect
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

    public interface IBenchmarkDoubleAspect
    {
        [Aspect<BenchmarkAspect>]
        void Run();
        
        [Aspect<AsyncBenchmarkAspect>]
        void AsyncRun();
    }

    public class BenchmarkDoubleAspectImpl : IBenchmarkDoubleAspect
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