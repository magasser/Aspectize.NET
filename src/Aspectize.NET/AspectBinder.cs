using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using Aspectize.NET.Extensions;

using Castle.DynamicProxy;

namespace Aspectize.NET;

public sealed class AspectBinder : IAspectBinder
{
    private readonly IProxyGenerator _proxyGenerator;
    private readonly IReadOnlyDictionary<Type, IAspect> _aspects;

    public AspectBinder(IAspectConfiguration configuration, IProxyGenerator proxyGenerator)
    {
        _aspects = configuration?.Aspects.ToDictionary(aspect => aspect.GetType(), aspect => aspect)
                ?? throw new ArgumentNullException(nameof(configuration));
        _proxyGenerator = proxyGenerator ?? throw new ArgumentNullException(nameof(proxyGenerator));
    }

    /// <inheritdoc />
    public object Bind(object target, Type targetInterface)
    {
        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        if (targetInterface is null)
        {
            throw new ArgumentNullException(nameof(targetInterface));
        }

        if (!targetInterface.IsInterface)
        {
            throw new ArgumentException("The target interface must be an interface type.", nameof(targetInterface));
        }

        return BindCore(target, targetInterface);
    }

    /// <inheritdoc />
    public T Bind<T>(T target) where T : class
    {
        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        var targetInterface = typeof(T);

        if (!targetInterface.IsInterface)
        {
            throw new ArgumentException($"The generic type '{nameof(T)}' must be an interface type.", nameof(target));
        }
        
        return (BindCore(target, targetInterface) as T)!;
    }

    public object BindCore(object target, Type targetInterface)
    {
        var targetType = target.GetType();
        
        var interfaces = targetType.GetInterfaces();

        if (!interfaces.Any(i => i == targetInterface))
        {
            throw new ArgumentException("The target must implement the target interface.", nameof(target));
        }
        
        var additionalInterfaces = interfaces.Except(targetInterface.GetInterfaces()).ToArray();

        var attributes = targetType.GetAspectAttributes();

        if (attributes.Count is 0)
        {
            return target;
        }

        var interceptor = CreateAspectInterceptor(targetType, attributes);

        var options = new ProxyGenerationOptions { Hook = new AspectProxyGenerationHook() };

        return _proxyGenerator.CreateInterfaceProxyWithTarget(
            interfaceToProxy: targetInterface,
            additionalInterfacesToProxy: additionalInterfaces,
            target,
            options,
            interceptor);
    }

    private IInterceptor CreateAspectInterceptor(Type targetType, IReadOnlyList<AspectAttribute> attributes)
    {
        var aspects = new List<IAspect>();

        foreach (var attribute in attributes)
        {
            if (!_aspects.TryGetValue(attribute.AspectType, out var aspect))
            {
                Debug.Fail($"Failed to find aspect for type '{attribute.AspectType.FullName}'.");
            }

            aspects.Add(aspect);
        }

        return new AspectInterceptor(targetType, aspects);
    }
}