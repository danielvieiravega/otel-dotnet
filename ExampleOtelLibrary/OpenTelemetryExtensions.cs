using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MassTransit.Monitoring;
using MassTransit.Logging;
using ExampleOtelLibrary.Processors;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace ExampleOtelLibrary;

public static class OpenTelemetryExtensions
{
    /// <summary>
    /// Adds OpenTelemetry to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
    /// <param name="configuration">Represents a set of key/value application configuration properties.</param>
    public static IServiceCollection AddExampleMetricsAndTracingOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var settings = configuration.GetSection(nameof(OpenTelemetryConfiguration)).Get<OpenTelemetryConfiguration>();

        if (settings == null) return services;

        services
            .AddOpenTelemetry()
            .ConfigureResource(x =>
            {
                x.AddTelemetrySdk();
                if (!string.IsNullOrWhiteSpace(settings.ServiceName))
                    x.AddService(settings.ServiceName, serviceVersion: settings.ServiceVersion);
            })
            .WithMetrics(metricsProviderBuilder =>
            {
                metricsProviderBuilder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddProcessInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter(InstrumentationOptions.MeterName) //MassTransit Observability
                    .AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(settings.Endpoint);
                    });

                if (settings.Debug)
                    metricsProviderBuilder.AddConsoleExporter();

                foreach (var meterName in settings.MeterNames)
                {
                    metricsProviderBuilder.AddMeter(meterName);
                }
            })
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .AddCustomProcessors()
                    .AddAspNetCoreInstrumentation(x => x.RecordException = true)
                    .AddHttpClientInstrumentation(x => x.RecordException = true)
                    .AddGrpcClientInstrumentation()
                    .AddGrpcCoreInstrumentation()
                    .AddElasticsearchClientInstrumentation()
                    .AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(settings.Endpoint);
                    })
                    .AddSource(DiagnosticHeaders.DefaultListenerName)
                    .AddSqlClientInstrumentation(options =>
                    {
                        options.EnableConnectionLevelAttributes = true;
                        options.SetDbStatementForStoredProcedure = true;
                        options.SetDbStatementForText = true;
                        options.RecordException = true;
                    })
                    .AddEntityFrameworkCoreInstrumentation(o =>
                    {
                        o.SetDbStatementForText = true;
                    })
                    .SetSampler(new TraceIdRatioBasedSampler(settings.SampleRate));

                if (settings.Debug)
                    tracerProviderBuilder.AddConsoleExporter();

            });

        //TODO: Ver se precisa mesmo
        // Register the default Meter so it can be injected into other components (eg controllers)
        //if (!string.IsNullOrWhiteSpace(settings.MetricsDataset))
        //    services.AddSingleton(new Meter(settings.MetricsDataset));

        services.AddHttpContextAccessor();

        return services;
    }

    private static TracerProviderBuilder AddCustomProcessors(this TracerProviderBuilder tracerProviderBuilder)
    {
        return tracerProviderBuilder
                    .AddProcessor<CorrelationIdProcessor>()
                    .AddProcessor<EnvironmentProcessor>()
                    .AddProcessor<BaggageSpanProcessor>();
    }

    /// <summary>
    /// Adds a delegate for configuring the provided <see cref="ILoggingBuilder"/>. This may be called multiple times.
    /// </summary>
    /// <param name="builder">The <see cref="IWebHostBuilder" /> to configure.</param>
    /// <param name="configuration">Represents a set of key/value application configuration properties.</param>
    /// <returns>The <see cref="IWebHostBuilder"/></returns>
    public static ILoggingBuilder AddExampleLogging(this ILoggingBuilder builder, IConfiguration configuration)
    {
        var settings = configuration.GetSection(nameof(OpenTelemetryConfiguration))
           .Get<OpenTelemetryConfiguration>();

        if (settings == null) return builder;

        builder.ClearProviders();
        builder.AddConsole();
        builder.AddOpenTelemetry(options =>
        {
            if (settings.Debug)
                options.AddConsoleExporter();

            options.AddOtlpExporter(otlpOptions =>
            {
                otlpOptions.Endpoint = new Uri(settings.Endpoint);
            });

            if (!string.IsNullOrWhiteSpace(settings.ServiceName))
                options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(settings.ServiceName, serviceVersion: settings.ServiceVersion));
        });

        return builder;
    }
}