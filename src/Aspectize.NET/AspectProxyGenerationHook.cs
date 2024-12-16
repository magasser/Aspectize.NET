using System;
using System.Reflection;

using Aspectize.NET.Extensions;

using Castle.DynamicProxy;

namespace Aspectize.NET;

internal sealed class AspectProxyGenerationHook : IProxyGenerationHook
{
    /// <inheritdoc />
    public void MethodsInspected()
    {
        // Do nothing
    }

    /// <inheritdoc />
    public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
    {
        // Do nothing
    }

    /// <inheritdoc />
    public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
    {
        return methodInfo.HasAnyAspect();
    }
}