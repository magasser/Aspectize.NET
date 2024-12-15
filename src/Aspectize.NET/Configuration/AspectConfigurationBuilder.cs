using System;
using System.Collections.Generic;

namespace Aspectize.NET;

public class AspectConfigurationBuilder
{
    private readonly List<IAspect> _aspects;

    private AspectConfigurationBuilder()
    {
        _aspects = new List<IAspect>();
    }

    public static AspectConfigurationBuilder Create()
    {
        return new AspectConfigurationBuilder();
    }

    public AspectConfigurationBuilder Use<TAspect>(TAspect aspect) where TAspect : IAspect
    {
        if (aspect is null)
        {
            throw new ArgumentNullException(nameof(aspect));
        }

        _aspects.Add(aspect);

        return this;
    }

    public IAspectConfiguration Build()
    {
        return new AspectConfiguration(_aspects);
    }
}