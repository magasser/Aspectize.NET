using System.Threading.Tasks;

namespace Aspectize.NET;

public interface IAspect { }

public interface IBeforeAspect : IAspect
{
    void Before(IBeforeInvocationContext context);
}

public interface IAfterAspect : IAspect
{
    void After(IAfterInvocationContext context);
}

public interface IAsyncBeforeAspect : IAspect
{
    Task BeforeAsync(IBeforeInvocationContext context);
}

public interface IAsyncAfterAspect : IAspect
{
    Task AfterAsync(IAfterInvocationContext context);
}