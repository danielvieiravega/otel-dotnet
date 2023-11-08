using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using OpenTelemetry;

namespace ExampleOtelLibrary.Processors;

internal sealed class CorrelationIdProcessor : BaseProcessor<Activity>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CorrelationIdAttribute = "correlation.id";

    public CorrelationIdProcessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override void OnStart(Activity data)
    {
        var correlationIdValues = _httpContextAccessor.HttpContext?.Request.Headers["X-MY-CORRELATION-ID"];
        string correlationId;
        var correlationIdFromActivity = data.GetTagItem(CorrelationIdAttribute);
        var correlationIdFromBaggage = Baggage.GetBaggage(CorrelationIdAttribute);

        if (correlationIdValues.HasValue && correlationIdValues.Value.Any())
            correlationId = correlationIdValues.Value.First();
        else if (correlationIdFromActivity != null)
            correlationId = correlationIdFromActivity.ToString()!;
        else if (correlationIdFromBaggage != null)
            correlationId = correlationIdFromBaggage.ToString()!;
        else
            correlationId = Guid.NewGuid().ToString();

        data.SetTag(CorrelationIdAttribute, correlationId);
        Baggage.SetBaggage(CorrelationIdAttribute, correlationId);
    }
}
