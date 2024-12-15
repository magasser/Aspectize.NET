using System;

namespace Aspectize.NET;

public interface IAspectBinder
{
    object Bind(object target, Type targetType);

    T Bind<T>(T target) where T : class;
}