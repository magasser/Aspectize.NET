using System;
using System.Collections.Generic;

namespace Aspectize.NET;

public interface IAspectProvider
{
    T GetAspect<T>() where T : IAspect;

    IAspect GetAspect(Type aspectType);

    IAspect GetAspect(AspectAttribute aspectAttribute);

    IReadOnlyList<IAspect> GetAspects();
}