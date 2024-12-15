using System;
using System.Linq;

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
            var originalProvider = services.BuildServiceProvider();

            var pairs = services.Where(descriptor => descriptor.ServiceType.IsInterface)
                                .Select(
                                    descriptor => (Descriptor: descriptor,
                                                   AspectAttributes: descriptor.ServiceType.GetAspectAttributes()))
                                .Where(pair => pair.AspectAttributes.Any())
                                .ToList();

            foreach (var descriptor in pairs.SelectMany(
                         pair => pair.AspectAttributes.Select(
                             attribute => ServiceDescriptor.Singleton(typeof(IAspect), attribute.AspectType))))
            {
                services.Add(descriptor);
            }

            services.AddSingleton(
                        provider =>
                        {
                            var aspects = provider.GetServices<IAspect>();

                            var builder = AspectConfigurationBuilder.Create();

                            foreach (var aspect in aspects)
                            {
                                builder.Use(aspect);
                            }

                            return builder.Build();
                        })
                    .AddSingleton<IAspectBinder, AspectBinder>();

            foreach (var descriptor in pairs.Select(pair => pair.Descriptor))
            {
                services.AddSingleton(
                    descriptor.ServiceType,
                    provider =>
                    {
                        var binder = provider.GetRequiredService<IAspectBinder>();
                        var target = originalProvider.GetRequiredService(descriptor.ServiceType);

                        return binder.Bind(target, descriptor.ServiceType);
                    });
            }
        }
    }
}