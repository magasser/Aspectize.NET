using System;
using System.Linq;

using Castle.DynamicProxy;

using Microsoft.Extensions.DependencyInjection;

namespace Aspectize.NET.Extensions.DependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void UseAspectize(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // TODO: Find a better way to resolve the original target

            // TODO (MaGas): Need to find a better way to resolve the original target
            var originalProvider = services.BuildServiceProvider();

            var pairs = services.Where(descriptor => descriptor.ServiceType.IsInterface)
                                .Select(
                                    descriptor => (Descriptor: descriptor,
                                                   AspectAttributes: descriptor.ServiceType.GetAspectAttributes()))
                                .Where(pair => pair.AspectAttributes.Count != 0)
                                .ToList();

            var aspectServiceDescriptors =
                pairs.SelectMany(pair => pair.AspectAttributes)
                     .Distinct()
                     .Select(
                         attribute =>
                             ServiceDescriptor.Singleton(
                                 typeof(IAspect),
                                 attribute.AspectType))
                     .ToList();

            foreach (var descriptor in aspectServiceDescriptors)
            {
                services.Add(descriptor);
            }

            services.AddSingleton<IAspectConfiguration>(
                        provider => new AspectConfiguration(provider.GetServices<IAspect>()))
                    .AddSingleton<IAspectBinder, AspectBinder>()
                    .AddSingleton<IProxyGenerator, ProxyGenerator>();

            foreach (var targetDescriptor in pairs.Select(pair => pair.Descriptor))
            {
                services.Remove(targetDescriptor);
                services.AddSingleton(
                    targetDescriptor.ServiceType,
                    provider =>
                    {
                        var binder = provider.GetRequiredService<IAspectBinder>();
                        var target = originalProvider.GetRequiredService(targetDescriptor.ServiceType);

                        return binder.Bind(target, targetDescriptor.ServiceType);
                    });
            }
        }
    }
}