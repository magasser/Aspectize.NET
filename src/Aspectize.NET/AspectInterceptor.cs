using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Aspectize.NET.Extensions;

using Castle.DynamicProxy;

namespace Aspectize.NET;

internal sealed class AspectInterceptor : IInterceptor
{
    private readonly IReadOnlyDictionary<MethodInfo, DelegateType> _methodTypes;
    private readonly IReadOnlyDictionary<MethodInfo, IReadOnlyList<IAspect>> _methodAspects;

    public AspectInterceptor(Type targetType, IEnumerable<IAspect> aspects)
    {
        if (targetType is null)
        {
            throw new ArgumentNullException(nameof(targetType));
        }

        var enumeratedAspects = aspects?.ToList() ?? throw new ArgumentNullException(nameof(aspects));

        var methods = targetType.GetMethods()
                                .Concat(targetType.GetInterfaces().SelectMany(i => i.GetMethods()))
                                .ToList();

        _methodTypes = methods.ToDictionary(method => method, GetDelegateType);
        _methodAspects = methods.ToDictionary(
            method => method,
            method => GetOrderedAspectsForMethod(method, enumeratedAspects));
    }

    /// <inheritdoc />
    public void Intercept(IInvocation invocation)
    {
        if (invocation is null)
        {
            throw new ArgumentNullException(nameof(invocation));
        }
        
        invocation.Proceed();

        var delegateType = _methodTypes[invocation.Method];

        switch (delegateType)
        {
            case DelegateType.Action or DelegateType.Func:
                InterceptSynchronous(invocation);
                break;
            case DelegateType.AsyncAction:
                InterceptAsyncAction(invocation);
                break;
            case DelegateType.AsyncFunc:
                InterceptAsyncFunc(invocation);
                break;
        }
    }

    private void InterceptSynchronous(IInvocation invocation)
    {
        InterceptCore(new InvocationContext(invocation), invocation.CaptureProceedInfo());
    }

    private void InterceptAsyncAction(IInvocation invocation)
    {
        invocation.ReturnValue = InterceptCoreAsync(new InvocationContext(invocation), invocation.CaptureProceedInfo());
    }

    private void InterceptAsyncFunc(IInvocation invocation)
    {
        var sourceType = typeof(TaskCompletionSource<>)
            .MakeGenericType(invocation.Method.ReturnType.GetGenericArguments()[0]);

        var source = Activator.CreateInstance(sourceType);

        invocation.ReturnValue = sourceType.GetProperty("Task")!.GetValue(source, null);

        _ = InterceptCoreAsync(new InvocationContext(invocation), invocation.CaptureProceedInfo())
            .ContinueWith(_ => { sourceType.GetMethod("SetResult")!.Invoke(source, [invocation.ReturnValue]); });
    }

    private void InterceptCore(IInvocationContext context, IInvocationProceedInfo proceedInfo)
    {
        /*var methodAspects = _methodAspects[context.Method];

        foreach (var aspect in methodAspects)
        {
            aspect.Before(context);

            switch (SynchronizationContext.Current)
            {
                case null:
                    aspect.BeforeAsync(context).GetAwaiter().GetResult();
                    break;
                default:
                    Task.Run(() => aspect.BeforeAsync(context)).GetAwaiter().GetResult();
                    break;
            }
        }*/

        proceedInfo.Invoke();

        /*foreach (var aspect in methodAspects)
        {
            aspect.After(context);

            switch (SynchronizationContext.Current)
            {
                case null:
                    aspect.AfterAsync(context).GetAwaiter().GetResult();
                    break;
                default:
                    Task.Run(() => aspect.AfterAsync(context)).GetAwaiter().GetResult();
                    break;
            }
        }*/
    }

    private async Task InterceptCoreAsync(IInvocationContext context, IInvocationProceedInfo proceedInfo)
    {
        var methodAspects = _methodAspects[context.Method];

        foreach (var aspect in methodAspects)
        {
            // ReSharper disable once MethodHasAsyncOverload
            aspect.Before(context);

            await aspect.BeforeAsync(context).ConfigureAwait(false);
        }

        proceedInfo.Invoke();

        await ((Task)context.ReturnValue).ConfigureAwait(false);

        foreach (var aspect in methodAspects)
        {
            // ReSharper disable once MethodHasAsyncOverload
            aspect.After(context);

            await aspect.AfterAsync(context).ConfigureAwait(false);
        }
    }

    private static DelegateType GetDelegateType(MethodInfo method)
    {
        return method.ReturnType switch
        {
            var t when t == typeof(void) => DelegateType.Action,
            var t when !typeof(Task).IsAssignableFrom(t) => DelegateType.Func,
            { IsGenericType: false } => DelegateType.AsyncAction,

            _ => DelegateType.AsyncFunc
        };
    }

    private static IReadOnlyList<IAspect> GetOrderedAspectsForMethod(
        MethodInfo method,
        IReadOnlyList<IAspect> aspects)
    {
        var pairs = new List<(AspectAttribute Attribute, IAspect Aspect)>();

        foreach (var attribute in method.DeclaringType!.GetCustomAttributes<AspectAttribute>()
                                        .Concat(method.GetCustomAttributes<AspectAttribute>())
                                        .Distinct())
        {
            var aspect = aspects.FirstOrDefault(aspect => aspect.GetType() == attribute.AspectType);

            if (aspect is null)
            {
                Debug.Fail("The aspect on target type '{TargetType}' of type '{AspectType}' was not found.");
                continue;
            }

            pairs.Add((attribute, aspect));
        }

        return pairs.OrderBy(pair => pair.Attribute.Order).Select(pair => pair.Aspect).ToList();
    }

    private enum DelegateType
    {
        Action = 1,
        Func = 2,
        AsyncAction = 3,
        AsyncFunc = 4
    }
}