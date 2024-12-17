using System;
using System.Collections.Generic;

namespace Aspectize.NET;

internal sealed class ConfigurationAspectProvider : AspectProvider
{
    private readonly IAspectConfiguration _configuration;

    public ConfigurationAspectProvider(IAspectConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <inheritdoc />
    public override IReadOnlyList<IAspect> GetAspects()
    {
        return _configuration.Aspects;
    }
}