using System;

namespace Aspectize.NET;

public interface IAspectConfiguration
{
    IAspect[] Aspects { get; }

    IAspect GetAspect<TAspect>() where TAspect : IAspect;

    IAspect GetAspect(Type aspectType);
}