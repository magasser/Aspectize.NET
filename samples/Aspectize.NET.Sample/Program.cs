using Aspectize.NET;
using Aspectize.NET.Sample.Domain;

using Castle.DynamicProxy;

var console = new ConsoleWrapper();

var configuration = AspectConfigurationBuilder.Create()
                                              .Use(new ConsoleLogAspect(console))
                                              .Use(new AsyncConsoleLogAspect(console))
                                              .Build();

var aspectBinder = new AspectBinder(configuration.Provider, new ProxyGenerator());

var sampleWithAspect = aspectBinder.Bind<ISampleInterface2>(new SampleImplementation());

sampleWithAspect.Method();
sampleWithAspect.Call();
sampleWithAspect.CallWithReturn();
await sampleWithAspect.CallAsync();
await sampleWithAspect.CallWithReturnAsync();

Console.ReadKey();