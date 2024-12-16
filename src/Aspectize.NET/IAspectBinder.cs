using System;

namespace Aspectize.NET;

public interface IAspectBinder
{
    object Bind(object target, Type targetInterface);

    T Bind<T>(T target) where T : class;
}