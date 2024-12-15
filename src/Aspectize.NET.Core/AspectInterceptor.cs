using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Aspectize.NET.Core.Domain;
using Aspectize.NET.Core.Extensions;

using Castle.DynamicProxy;

namespace Aspectize.NET.Core
{
    internal sealed class AspectInterceptor : IInterceptor
    {
        private readonly IAspect _aspect;
        private readonly bool _interceptAll;
        private readonly Dictionary<MethodInfo, bool> _shouldIntercept;

        public AspectInterceptor(Type targetType, IAspect aspect)
        {
            if (targetType is null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            _aspect = aspect ?? throw new ArgumentNullException(nameof(aspect));

            _interceptAll = targetType.GetCustomAttributes<AspectAttribute>()
                                      .Any(attribute => attribute.AspectType == aspect.GetType());

            _shouldIntercept = new Dictionary<MethodInfo, bool>(
                targetType.GetMethods().ToDictionary(method => method, ShouldInterceptCore));
        }

        /// <inheritdoc />
        public void Intercept(IInvocation invocation)
        {
            if (invocation is null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            if (!ShouldIntercept(invocation))
            {
                invocation.Proceed();
                return;
            }

            var context = new InvocationContext(invocation);

            var delegateType = invocation.Method.GetDelegateType();

            if (delegateType is DelegateType.Action || delegateType is DelegateType.Func)
            {
                InterceptCore(context);
                return;
            }

            var proceedInfo = invocation.CaptureProceedInfo();

            if (delegateType is DelegateType.AsyncAction)
            {
                invocation.ReturnValue = InterceptCoreAsync(context, proceedInfo);
                return;
            }

            var sourceType = typeof(TaskCompletionSource<>)
                .MakeGenericType(invocation.Method.ReturnType.GetGenericArguments()[0]);

            var source = Activator.CreateInstance(sourceType);

            invocation.ReturnValue = sourceType.GetProperty("Task")!.GetValue(source, null);

            _ = InterceptCoreAsync(context, proceedInfo)
                .ContinueWith(
                    _ => { sourceType.GetMethod("SetResult")!.Invoke(source, new[] { invocation.ReturnValue }); });
        }

        private void InterceptCore(InvocationContext context)
        {
            switch (_aspect)
            {
                case IBeforeAspect beforeAspect:
                    beforeAspect.Before(context);
                    break;
                case IAsyncBeforeAspect asyncBeforeAspect when SynchronizationContext.Current is null:
                    asyncBeforeAspect.BeforeAsync(context).GetAwaiter().GetResult();
                    break;
                case IAsyncBeforeAspect asyncBeforeAspect:
                    Task.Run(() => asyncBeforeAspect.BeforeAsync(context)).GetAwaiter().GetResult();
                    break;
            }

            context.Invocation.Proceed();

            switch (_aspect)
            {
                case IAfterAspect afterAspect:
                    afterAspect.After(context);
                    break;
                case IAsyncAfterAspect asyncAfterAspect when SynchronizationContext.Current is null:
                    asyncAfterAspect.AfterAsync(context).GetAwaiter().GetResult();
                    break;
                case IAsyncAfterAspect asyncAfterAspect:
                    Task.Run(() => asyncAfterAspect.AfterAsync(context)).GetAwaiter().GetResult();
                    break;
            }
        }

        private async Task InterceptCoreAsync(InvocationContext context, IInvocationProceedInfo proceedInfo)
        {
            switch (_aspect)
            {
                case IBeforeAspect beforeAspect:
                    beforeAspect.Before(context);
                    break;
                case IAsyncBeforeAspect asyncBeforeAspect:
                    await asyncBeforeAspect.BeforeAsync(context).ConfigureAwait(false);
                    break;
            }

            proceedInfo.Invoke();

            await ((Task)context.ReturnValue).ConfigureAwait(false);

            switch (_aspect)
            {
                case IAfterAspect afterAspect:
                    afterAspect.After(context);
                    break;
                case IAsyncAfterAspect asyncAfterAspect:
                    await asyncAfterAspect.AfterAsync(context).ConfigureAwait(false);
                    break;
            }
        }

        private bool ShouldIntercept(IInvocation invocation)
        {
            return _interceptAll
                || (_shouldIntercept.TryGetValue(invocation.Method, out var shouldIntercept) && shouldIntercept);
        }

        private bool ShouldInterceptCore(MethodInfo method)
        {
            return method.GetAspectAttributes()
                         .Any(attribute => attribute.AspectType == _aspect.GetType());
        }
    }
}