namespace Aspectize.NET;

public interface IAspectConfiguration
{
    IAspect[] Aspects { get; }

    IAspectProvider Provider { get; }
}