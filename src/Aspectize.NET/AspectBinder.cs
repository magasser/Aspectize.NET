using System;
using System.Linq;

using Aspectize.NET.Extensions;

namespace Aspectize.NET;

public sealed class AspectBinder : IAspectBinder
{
    private readonly IAspectConfiguration _configuration;
    private readonly AspectProxyGenerator _proxyGenerator;

    public AspectBinder(IAspectConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        _proxyGenerator = new AspectProxyGenerator();
    }

    /// <inheritdoc />
    public object Bind(object target, Type targetType)
    {
        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        if (targetType is null)
        {
            throw new ArgumentNullException(nameof(targetType));
        }

        var aspects = GetAspects(targetType);

        return aspects.Length is 0 ? target : _proxyGenerator.CreateProxy(targetType, target, aspects);
    }

    /// <inheritdoc />
    public T Bind<T>(T target) where T : class
    {
        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        return (Bind(target, typeof(T)) as T)!;
    }

    private IAspect[] GetAspects(Type targetType)
    {
        return targetType.GetMethods()
                         .SelectMany(method => method.GetAspectAttributes())
                         .Distinct()
                         .Select(attribute => _configuration.GetAspect(attribute.AspectType))
                         .ToArray();
    }
}