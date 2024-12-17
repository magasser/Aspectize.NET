using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Castle.DynamicProxy;

namespace Aspectize.NET;

internal sealed class AspectInterceptor : IInterceptor
{
    private readonly IReadOnlyDictionary<MethodInfo, DelegateType> _methodTypes;
    private readonly IReadOnlyDictionary<MethodInfo, IReadOnlyList<IAspect>> _methodAspects;

    public AspectInterceptor(Type targetType, IAspectProvider aspectProvider)
    {
        if (targetType is null)
        {
            throw new ArgumentNullException(nameof(targetType));
        }

        if (aspectProvider is null)
        {
            throw new ArgumentNullException(nameof(aspectProvider));
        }

        var methods = targetType.GetMethods()
                                .Concat(targetType.GetInterfaces().SelectMany(i => i.GetMethods()))
                                .ToList();

        _methodTypes = methods.ToDictionary(method => method, GetDelegateType);
        _methodAspects = methods.ToDictionary(
            method => method,
            method => GetOrderedAspectsForMethod(method, aspectProvider));
    }

    /// <inheritdoc />
    public void Intercept(IInvocation invocation)
    {
        if (invocation is null)
        {
            throw new ArgumentNullException(nameof(invocation));
        }

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

    private void InterceptCore(InvocationContext context, IInvocationProceedInfo proceedInfo)
    {
        var methodAspects = _methodAspects[context.Method];

        foreach (var aspect in methodAspects)
        {
            switch (aspect)
            {
                case Aspect syncAspect:
                    syncAspect.Before(context);
                    break;
                case AsyncAspect asyncAspect when SynchronizationContext.Current is null:
                    asyncAspect.BeforeAsync(context).GetAwaiter().GetResult();
                    break;
                case AsyncAspect asyncAspect:
                    Task.Run(() => asyncAspect.BeforeAsync(context)).GetAwaiter().GetResult();
                    break;
            }
        }

        proceedInfo.Invoke();

        context.SynchronousReturnValue = context.ReturnValue;

        foreach (var aspect in methodAspects)
        {
            switch (aspect)
            {
                case Aspect syncAspect:
                    syncAspect.After(context);
                    break;
                case AsyncAspect asyncAspect when SynchronizationContext.Current is null:
                    asyncAspect.AfterAsync(context).GetAwaiter().GetResult();
                    break;
                case AsyncAspect asyncAspect:
                    Task.Run(() => asyncAspect.AfterAsync(context)).GetAwaiter().GetResult();
                    break;
            }
        }
    }

    private async Task InterceptCoreAsync(InvocationContext context, IInvocationProceedInfo proceedInfo)
    {
        var methodAspects = _methodAspects[context.Method];

        foreach (var aspect in methodAspects)
        {
            switch (aspect)
            {
                case Aspect syncAspect:
                    syncAspect.Before(context);
                    break;
                case AsyncAspect asyncAspect:
                    await asyncAspect.BeforeAsync(context).ConfigureAwait(false);
                    break;
            }
        }

        proceedInfo.Invoke();

        var task = (Task)context.ReturnValue!;

        await task.ConfigureAwait(false);

        context.AsynchronousReturnValue = context.Method.ReturnType.IsGenericType
                                              ? context.Method.ReturnType.GetProperty("Result")!.GetValue(task, null)
                                              : null;

        foreach (var aspect in methodAspects)
        {
            switch (aspect)
            {
                case Aspect syncAspect:
                    syncAspect.After(context);
                    break;
                case AsyncAspect asyncAspect:
                    await asyncAspect.AfterAsync(context).ConfigureAwait(false);
                    break;
            }
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
        IAspectProvider provider)
    {
        return method.DeclaringType!.GetCustomAttributes<AspectAttribute>()
                     .Concat(method.GetCustomAttributes<AspectAttribute>())
                     .Distinct()
                     .OrderBy(attribute => attribute.Order)
                     .Select(provider.GetAspect)
                     .ToList();
    }

    private enum DelegateType
    {
        Action = 1,
        Func = 2,
        AsyncAction = 3,
        AsyncFunc = 4
    }
}