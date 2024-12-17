using System;
using System.Collections.Generic;
using System.Linq;

namespace Aspectize.NET;

public abstract class AspectProvider : IAspectProvider
{
    /// <inheritdoc />
    public T GetAspect<T>() where T : IAspect
    {
        return (T)GetAspect(typeof(T));
    }

    /// <inheritdoc />
    public IAspect GetAspect(Type aspectType)
    {
        return GetAspects().First(aspect => aspect.GetType() == aspectType);
    }

    /// <inheritdoc />
    public IAspect GetAspect(AspectAttribute aspectAttribute)
    {
        return GetAspect(aspectAttribute.AspectType);
    }

    /// <inheritdoc />
    public abstract IReadOnlyList<IAspect> GetAspects();
}