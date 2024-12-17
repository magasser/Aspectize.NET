using System;
using System.Linq;

using Aspectize.NET.Extensions;

using Castle.DynamicProxy;

namespace Aspectize.NET;

public sealed class AspectBinder : IAspectBinder
{
    private readonly IAspectProvider _aspectProvider;
    private readonly IProxyGenerator _proxyGenerator;

    public AspectBinder(IAspectProvider aspectProvider, IProxyGenerator proxyGenerator)
    {
        _aspectProvider = aspectProvider ?? throw new ArgumentNullException(nameof(aspectProvider));
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

        if (interfaces.All(i => i != targetInterface))
        {
            throw new ArgumentException("The target must implement the target interface.", nameof(target));
        }

        var additionalInterfaces = interfaces.Except(targetInterface.GetInterfaces()).ToArray();

        var attributes = targetType.GetAspectAttributes();

        if (attributes.Count is 0)
        {
            return target;
        }

        var interceptor = new AspectInterceptor(targetType, _aspectProvider);

        var options = new ProxyGenerationOptions { Hook = new AspectProxyGenerationHook() };

        return _proxyGenerator.CreateInterfaceProxyWithTarget(
            interfaceToProxy: targetInterface,
            additionalInterfacesToProxy: additionalInterfaces,
            target,
            options,
            interceptor);
    }
}