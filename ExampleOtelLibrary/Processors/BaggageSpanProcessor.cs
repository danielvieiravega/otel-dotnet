using System.Diagnostics;
using OpenTelemetry;

namespace ExampleOtelLibrary.Processors;

/// <summary>
/// Span processor that adds <see cref="Baggage"/> fields to every new span
/// </summary>
internal sealed class BaggageSpanProcessor : BaseProcessor<Activity>
{
    public override void OnStart(Activity data)
    {
        foreach (var item in Baggage.Current)
            data.SetTag(item.Key, item.Value);
    }
}