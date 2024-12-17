using System.Threading.Tasks;

namespace Aspectize.NET;

public interface IAspect
{
    bool IsActive { get; }

    /*public void Before(IInvocationContext context);

    public void After(IInvocationContext context);

    public Task BeforeAsync(IInvocationContext context);

    public Task AfterAsync(IInvocationContext context);*/
}

public abstract class Aspect : IAspect
{
    /// <inheritdoc />
    public virtual bool IsActive => true;

    public virtual void Before(IInvocationContext context) { }

    public virtual void After(IInvocationContext context) { }
}

public abstract class AsyncAspect : IAspect
{
    /// <inheritdoc />
    public virtual bool IsActive => true;

    public virtual Task BeforeAsync(IInvocationContext context)
    {
        return Task.CompletedTask;
    }

    public virtual Task AfterAsync(IInvocationContext context)
    {
        return Task.CompletedTask;
    }
}