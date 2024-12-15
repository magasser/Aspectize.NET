// See https://aka.ms/new-console-template for more information

using Aspectize.NET.Extensions.DependencyInjection.Extensions;
using Aspectize.NET.Sample.Domain;

using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddSingleton<ISampleInterface, SampleImplementation>()
        .AddSingleton<IConsoleWrapper, ConsoleWrapper>()
        .UseAspectize();

var provider = services.BuildServiceProvider();

var sampleWithAspect = provider.GetRequiredService<ISampleInterface>();

sampleWithAspect.Call();
await sampleWithAspect.CallAsync();
await sampleWithAspect.CallWithReturnAsync();

Console.ReadKey();