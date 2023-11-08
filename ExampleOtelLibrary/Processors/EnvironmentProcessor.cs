using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;

namespace ExampleOtelLibrary.Processors;

internal sealed class EnvironmentProcessor : BaseProcessor<Activity>
{
    private readonly string? _environment;
    private const string EnvironmenAtttribute = "environment.name";

    public EnvironmentProcessor(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var hostingEnvironment = scope.ServiceProvider.GetService<IWebHostEnvironment>();
        _environment = hostingEnvironment?.EnvironmentName ?? "undefined";
    }

    public override void OnStart(Activity data)
    {
        data.SetTag(EnvironmenAtttribute, _environment);
        Baggage.SetBaggage(EnvironmenAtttribute, _environment);
    }
}