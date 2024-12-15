using System;

namespace Aspectize.NET;

[AttributeUsage(
    AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method,
    AllowMultiple = true)]
public class AspectAttribute : Attribute
{
    public AspectAttribute(Type aspectType)
    {
        if (aspectType is null)
        {
            throw new ArgumentNullException(nameof(aspectType));
        }

        if (!typeof(IAspect).IsAssignableFrom(aspectType))
        {
            throw new ArgumentException($"The aspect type must implement {nameof(IAspect)}.");
        }

        AspectType = aspectType;
    }

    public Type AspectType { get; }
}

[AttributeUsage(
    AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method,
    AllowMultiple = true)]
public class AspectAttribute<TAspect> : AspectAttribute where TAspect : IAspect
{
    /// <inheritdoc />
    public AspectAttribute()
        : base(typeof(TAspect)) { }
}