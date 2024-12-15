using System;
using System.Collections.Generic;
using System.Linq;

using Castle.DynamicProxy;

namespace Aspectize.NET.Core
{
    public class AspectProxyGenerator
    {
        private readonly ProxyGenerator _proxyGenerator;

        public AspectProxyGenerator()
        {
            _proxyGenerator = new ProxyGenerator();
        }

        public object CreateProxy(Type proxyType, object target, IEnumerable<IAspect> aspects)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (aspects is null)
            {
                throw new ArgumentNullException(nameof(aspects));
            }

            var interceptors = aspects.Select(aspect => new AspectInterceptor(proxyType, aspect))
                                      .OfType<IInterceptor>()
                                      .ToArray();

            return proxyType.IsInterface
                       ? _proxyGenerator.CreateInterfaceProxyWithTarget(proxyType, target, interceptors)
                       : _proxyGenerator.CreateClassProxyWithTarget(proxyType, target, interceptors);
        }
    }
}