using MassTransit;
using System.Data.SqlClient;
using System.Data;
using WebAPI;

namespace Worker;

public class MensagemConsumer : IConsumer<Mensagem>
{
    private readonly ILogger<MensagemConsumer> _logger;
    private readonly IConfiguration _configuration;


    public MensagemConsumer(ILogger<MensagemConsumer> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task Consume(ConsumeContext<Mensagem> context)
    {
        _logger.LogInformation("Recebida uma nova mensagem: {message}", context.Message.Name);

        using SqlConnection connection = new(_configuration.GetConnectionString("PessoasDb"));
        connection.Open();

        var nome = context.Message.Name;
        var id = Guid.NewGuid();
        var sql = "INSERT INTO Pessoas (Id, Nome) VALUES ( @Id, @Nome)";

        using SqlCommand command = new(sql, connection);
        command.Parameters.Add(new SqlParameter("@id", SqlDbType.UniqueIdentifier) { Value = id });
        command.Parameters.Add(new SqlParameter("@nome", SqlDbType.NVarChar) { Value = nome });

        int rowsAffected = command.ExecuteNonQuery();

        if (rowsAffected > 0)
        {
            _logger.LogInformation("Inserção bem-sucedida!");
        }
        else
        {
            _logger.LogError("Nenhuma linha inserida.");
        }

        await Task.CompletedTask;
    }
}
