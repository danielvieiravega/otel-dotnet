using OpenTelemetry.Trace;
using System.Diagnostics;

namespace WebAPI;

/// <summary>
/// Middleware responsável pora capturar de maneira centralizada as exceções não tratadas
/// e gravá-las em seu respectivo trace do OpenTelemetry
/// </summary>
public class OpenTelemetryMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<OpenTelemetryMiddleware> _logger;

    public OpenTelemetryMiddleware(RequestDelegate next, ILogger<OpenTelemetryMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context, IWebHostEnvironment env)
    {
        try
        {
            await _next(context);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Um erro aconteceu");
            Activity.Current?.RecordException(ex);
            throw;
        }
    }
}
