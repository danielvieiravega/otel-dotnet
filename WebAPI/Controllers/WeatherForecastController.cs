using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;


[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        // Crie uma instância do HttpClient (normalmente é melhor reutilizar a instância em vez de criar uma nova a cada vez)
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Faça uma solicitação GET para a URL desejada
                string apiUrl = "https://jsonplaceholder.typicode.com/todos";
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                // Verifique se a resposta foi bem-sucedida (código de status 200 OK)
                if (response.IsSuccessStatusCode)
                {
                    // Leia o conteúdo da resposta como uma string
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Imprima o JSON de resposta
                    Console.WriteLine(responseBody);
                }
                else
                {
                    Console.WriteLine($"A solicitação não foi bem-sucedida. Código de status: {response.StatusCode}");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Erro na solicitação HTTP: {e.Message}");
            }
        }

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}