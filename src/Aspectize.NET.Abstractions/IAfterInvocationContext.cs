using System;
using System.Reflection;

namespace Aspectize.NET;

public interface IAfterInvocationContext
{
    object Target { get; }

    Type TargetType { get; }

    MethodInfo Method { get; }

    object[] Arguments { get; }

    Type[] GenericArguments { get; }

    object ReturnValue { get; }
}