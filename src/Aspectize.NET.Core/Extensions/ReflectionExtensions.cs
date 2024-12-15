using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Aspectize.NET.Core.Domain;

namespace Aspectize.NET.Core.Extensions
{
    internal static class ReflectionExtensions
    {
        public static DelegateType GetDelegateType(this MethodInfo method)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            return method.ReturnType switch
            {
                var t when t == typeof(void) => DelegateType.Action,
                var t when !typeof(Task).IsAssignableFrom(t) => DelegateType.Func,
                { IsGenericType: false } => DelegateType.AsyncAction,

                _ => DelegateType.AsyncFunc
            };
        }

        // TODO: Improve these methods to support more than interfaces
        public static AspectAttribute[] GetAspectAttributes(this Type type)
        {
            if (!type.IsInterface)
            {
                return Array.Empty<AspectAttribute>();
            }

            var interfaceAttributes =
                type.GetCustomAttributes(typeof(AspectAttribute), inherit: false)
                    .ToAspectAttributes();

            var methodAttributes = type.GetMethods()
                                       .SelectMany(
                                           method => method.GetCustomAttributes(
                                               typeof(AspectAttribute),
                                               inherit: false))
                                       .ToAspectAttributes();

            return interfaceAttributes.Concat(methodAttributes).ToArray();
        }

        public static AspectAttribute[] GetAspectAttributes(this MethodInfo method)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (!method.DeclaringType!.IsInterface)
            {
                return Array.Empty<AspectAttribute>();
            }

            var interfaceAttributes = method.DeclaringType!
                                            .GetCustomAttributes(typeof(AspectAttribute), inherit: false)
                                            .ToAspectAttributes();

            var methodAttributes = method.GetCustomAttributes(
                                             typeof(AspectAttribute),
                                             inherit: false)
                                         .ToAspectAttributes();

            return interfaceAttributes.Concat(methodAttributes)
                                      .ToArray();
        }

        private static AspectAttribute[] ToAspectAttributes(this IEnumerable<object> attributes)
        {
            return attributes.OfType<AspectAttribute>().ToArray();
        }
    }
}