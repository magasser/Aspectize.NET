using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

namespace Aspectize.NET.Extensions.DependencyInjection
{
    internal sealed class ServiceAspectProvider : AspectProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceAspectProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <inheritdoc />
        public override IReadOnlyList<IAspect> GetAspects()
        {
            return _serviceProvider.GetServices<IAspect>().ToList();
        }
    }
}