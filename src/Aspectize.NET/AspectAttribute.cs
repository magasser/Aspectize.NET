using System;

namespace Aspectize.NET;

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = true)]
public class AspectAttribute : Attribute
{
    public AspectAttribute(Type aspectType)
    {
        if (aspectType is null)
        {
            throw new ArgumentNullException(nameof(aspectType));
        }

        if (aspectType == typeof(IAspect) || aspectType == typeof(Aspect))
        {
            throw new ArgumentException($"The aspect type must not be '{nameof(IAspect)}' or '{nameof(Aspect)}'.");
        }

        if (!typeof(IAspect).IsAssignableFrom(aspectType))
        {
            throw new ArgumentException($"The aspect type must implement '{nameof(IAspect)}'.");
        }

        AspectType = aspectType;
    }

    public Type AspectType { get; }

    public int Order { get; set; } = 0;
}

/// <inheritdoc />
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = true)]
public class AspectAttribute<TAspect> : AspectAttribute where TAspect : IAspect
{
    /// <inheritdoc />
    public AspectAttribute()
        : base(typeof(TAspect)) { }
}