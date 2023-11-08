using System.Diagnostics.Metrics;

namespace ExampleOtelLibrary;

public class OpenTelemetryConfiguration
{
    /// <summary>
    /// Default OTLP endpoint.
    /// </summary>
    public const string DefaultOTLPEndpoint = "http://otel-collector:4317";

    /// <summary>
    /// Default sample rate - sample 10%.
    /// </summary>
    public const double DefaultSampleRate = 0.1;

    /// <summary>
    /// Sets the value of the service.name resource attribute
    /// </summary>
    public string? ServiceName { get; set; }
    
    /// <summary>
    /// API endpoint to send telemetry data. Defaults to <see cref="DefaultOTLPEndpoint"/>.
    /// </summary>
    public string Endpoint { get; set; } = DefaultOTLPEndpoint;

    /// <summary>
    /// Optional version of service
    /// </summary>
    public string? ServiceVersion { get; set; }
    
    /// <summary>
    /// The desired probability of sampling. This must be between 0.0 and 1.0 
    /// Higher the value, higher is the probability of a given Activity to be sampled in.
    /// Defaults to <see cref="DefaultSampleRate"/>
    /// </summary>
    public double SampleRate { get; set; } = DefaultSampleRate;
    
    /// <summary>
    /// If set to true, enables the console span exporter for local debugging.
    /// </summary>
    public bool Debug { get; set; } = false;
    
    /// <summary>
    /// (Optional) Additional <see cref="Meter"/> names for generating metrics.
    /// <see cref="ServiceName"/> is configured as a meter name by default.
    /// </summary>
    public List<string> MeterNames { get; set; } = new List<string>();
}