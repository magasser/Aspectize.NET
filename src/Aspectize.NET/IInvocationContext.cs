using System;
using System.Reflection;

namespace Aspectize.NET;

public interface IInvocationContext
{
    object Target { get; }

    Type TargetType { get; }

    MethodInfo Method { get; }

    object[] Arguments { get; }

    public object? ReturnValue { get; }

    public object? SynchronousReturnValue { get; }

    public object? AsynchronousReturnValue { get; }

    object GetArgument(int index);

    void SetArgument(int index, object value);
}