namespace Worker
{
    public class WorkerService : BackgroundService
    {
        private readonly ILogger<WorkerService> _logger;

        public WorkerService(ILogger<WorkerService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Crie uma inst�ncia do HttpClient (normalmente � melhor reutilizar a inst�ncia em vez de criar uma nova a cada vez)
                //using (HttpClient client = new HttpClient())
                //{
                //    try
                //    {
                //        // Fa�a uma solicita��o GET para a URL desejada
                //        string apiUrl = "https://jsonplaceholder.typicode.com/todos";
                //        HttpResponseMessage response = await client.GetAsync(apiUrl);

                //        // Verifique se a resposta foi bem-sucedida (c�digo de status 200 OK)
                //        if (response.IsSuccessStatusCode)
                //        {
                //            // Leia o conte�do da resposta como uma string
                //            string responseBody = await response.Content.ReadAsStringAsync();

                //            // Imprima o JSON de resposta
                //            Console.WriteLine(responseBody);
                //        }
                //        else
                //        {
                //            Console.WriteLine($"A solicita��o n�o foi bem-sucedida. C�digo de status: {response.StatusCode}");
                //        }
                //    }
                //    catch (HttpRequestException e)
                //    {
                //        Console.WriteLine($"Erro na solicita��o HTTP: {e.Message}");
                //    }
                //}

                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //await Task.Delay(1000, stoppingToken);
            }
        }
    }
}