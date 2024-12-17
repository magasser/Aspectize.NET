using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aspectize.NET.Extensions;

internal static class ReflectionExtensions
{
    public static IReadOnlyList<AspectAttribute> GetAspectAttributes(this Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        return getAspectAttributes(type)
               .Concat(type.GetInterfaces().SelectMany(getAspectAttributes))
               .Distinct()
               .ToList();

        IEnumerable<AspectAttribute> getAspectAttributes(Type t)
        {
            return t.GetCustomAttributes<AspectAttribute>(inherit: false)
                    .Concat(
                        t.GetMethods()
                         .SelectMany(method => method.GetCustomAttributes<AspectAttribute>(inherit: false)));
        }
    }

    public static bool HasAspect(this MethodInfo method, IAspect aspect)
    {
        if (method is null)
        {
            throw new ArgumentNullException(nameof(method));
        }

        if (aspect is null)
        {
            throw new ArgumentNullException(nameof(aspect));
        }

        var aspectType = aspect.GetType();

        var baseMethod = method.GetBaseDefinition();

        return baseMethod.GetCustomAttributes<AspectAttribute>(inherit: false)
                         .Any(attribute => attribute.AspectType == aspectType)
            || (baseMethod.DeclaringType?.GetCustomAttributes<AspectAttribute>(inherit: false)
                          .Any(attribute => attribute.AspectType == aspectType)
             ?? false);
    }

    public static bool HasAnyAspect(this MethodInfo method)
    {
        if (method is null)
        {
            throw new ArgumentNullException(nameof(method));
        }

        var baseMethod = method.GetBaseDefinition();

        return baseMethod.GetCustomAttributes<AspectAttribute>(inherit: false).Any()
            || (baseMethod.DeclaringType?.GetCustomAttributes<AspectAttribute>(inherit: false).Any()
             ?? false);
    }
}