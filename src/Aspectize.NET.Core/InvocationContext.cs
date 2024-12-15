using System;
using System.Reflection;

using Castle.DynamicProxy;

namespace Aspectize.NET.Core
{
    internal sealed class InvocationContext : IAfterInvocationContext, IBeforeInvocationContext
    {
        public InvocationContext(IInvocation invocation)
        {
            Invocation = invocation ?? throw new ArgumentNullException(nameof(invocation));
        }

        public object Target => Invocation.InvocationTarget;

        public Type TargetType => Invocation.TargetType;

        public MethodInfo Method => Invocation.MethodInvocationTarget;

        public object[] Arguments => Invocation.Arguments;

        public Type[] GenericArguments => Invocation.GenericArguments;

        public object ReturnValue => Invocation.ReturnValue;

        internal IInvocation Invocation { get; }

        public object GetArgument(int index)
        {
            return Invocation.GetArgumentValue(index);
        }

        public void SetArgument(int index, object value)
        {
            Invocation.SetArgumentValue(index, value);
        }
    }
}