using System.Threading.Tasks;

namespace Aspectize.NET;

public interface IAspect
{
    bool IsActive { get; }

    void Before(IInvocationContext context);

    void After(IInvocationContext context);

    Task BeforeAsync(IInvocationContext context);

    Task AfterAsync(IInvocationContext context);
}

public abstract class Aspect : IAspect
{
    /// <inheritdoc />
    public virtual bool IsActive => true;

    /// <inheritdoc />
    public virtual void Before(IInvocationContext context) { }

    /// <inheritdoc />
    public virtual void After(IInvocationContext context) { }

    /// <inheritdoc />
    public virtual Task BeforeAsync(IInvocationContext context)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task AfterAsync(IInvocationContext context)
    {
        return Task.CompletedTask;
    }
}