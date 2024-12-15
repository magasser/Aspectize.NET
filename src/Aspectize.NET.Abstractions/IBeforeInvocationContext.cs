using System;
using System.Reflection;

namespace Aspectize.NET;

public interface IBeforeInvocationContext
{
    object Target { get; }

    Type TargetType { get; }

    MethodInfo Method { get; }

    object[] Arguments { get; }

    Type[] GenericArguments { get; }

    object GetArgument(int index);

    void SetArgument(int index, object value);
}