using System;
using System.Reflection;

using Castle.DynamicProxy;

namespace Aspectize.NET;

internal sealed class InvocationContext : IInvocationContext
{
    private readonly IInvocation _invocation;

    public InvocationContext(IInvocation invocation)
    {
        _invocation = invocation ?? throw new ArgumentNullException(nameof(invocation));
    }

    public object Target => _invocation.InvocationTarget;

    public Type TargetType => _invocation.TargetType;

    public MethodInfo Method => _invocation.Method;

    public object[] Arguments => _invocation.Arguments;

    public Type[] GenericArguments => _invocation.GenericArguments;

    public object? ReturnValue => _invocation.ReturnValue;

    /// <inheritdoc />
    public object? SynchronousReturnValue { get; internal set; }

    /// <inheritdoc />
    public object? AsynchronousReturnValue { get; internal set; }

    public object GetArgument(int index)
    {
        return _invocation.GetArgumentValue(index);
    }

    public void SetArgument(int index, object value)
    {
        _invocation.SetArgumentValue(index, value);
    }
}