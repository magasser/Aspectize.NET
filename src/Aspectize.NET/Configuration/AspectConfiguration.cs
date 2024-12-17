using System;
using System.Collections.Generic;
using System.Linq;

namespace Aspectize.NET;

public sealed class AspectConfiguration : IAspectConfiguration
{
    public AspectConfiguration(IEnumerable<IAspect> aspects)
    {
        Aspects = aspects?.ToArray() ?? throw new ArgumentNullException(nameof(aspects));
        Provider = new ConfigurationAspectProvider(this);
    }

    /// <inheritdoc />
    public IAspect[] Aspects { get; }

    /// <inheritdoc />
    public IAspectProvider Provider { get; }
}