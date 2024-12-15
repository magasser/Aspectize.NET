using Aspectize.NET;
using Aspectize.NET.Core;
using Aspectize.NET.Sample.Domain;

var console = new ConsoleWrapper();

var configuration = AspectConfigurationBuilder.Create()
                                              .Use(new ConsoleLogAspect(console))
                                              .Use(new AsyncConsoleLogAspect(console))
                                              .Build();

var aspectBinder = new AspectBinder(configuration);

var sampleWithAspect = aspectBinder.Bind<ISampleInterface>(new SampleImplementation());

sampleWithAspect.Call();
await sampleWithAspect.CallAsync();
await sampleWithAspect.CallWithReturnAsync();

Console.ReadKey();