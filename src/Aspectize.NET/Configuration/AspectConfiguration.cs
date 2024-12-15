using System;
using System.Collections.Generic;
using System.Linq;

namespace Aspectize.NET;

public sealed class AspectConfiguration : IAspectConfiguration
{
    public AspectConfiguration(IEnumerable<IAspect> aspects)
    {
        Aspects = aspects?.ToArray() ?? throw new ArgumentNullException(nameof(aspects));
    }

    /// <inheritdoc />
    public IAspect[] Aspects { get; }

    /// <inheritdoc />
    public IAspect GetAspect<TAspect>() where TAspect : IAspect
    {
        return GetAspect(typeof(TAspect));
    }

    /// <inheritdoc />
    public IAspect GetAspect(Type aspectType)
    {
        if (!typeof(IAspect).IsAssignableFrom(aspectType))
        {
            throw new ArgumentException(
                $"The aspect type must be of type '{typeof(IAspect).FullName}'.",
                nameof(aspectType));
        }

        return Aspects.FirstOrDefault(aspect => aspect.GetType() == aspectType)
            ?? throw new Exception($"The aspect of type '{aspectType.FullName}' was not found.");
    }
}