using ExampleOtelLibrary;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using WebAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add the example Otel library
builder.Services.AddExampleMetricsAndTracingOpenTelemetry(builder.Configuration);
builder.Logging.AddExampleLogging(builder.Configuration);

// Add Masstransit 
var masstransit = builder.Configuration.GetRequiredSection(nameof(MasstransitConfiguration)).Get<MasstransitConfiguration>();
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var broker = masstransit.Broker;
        cfg.Host(broker.Host, broker.Port, broker.VirtualHost, c =>
        {
            c.Username(broker.Username);
            c.Password(broker.Password);
        });

        //Producer configuration
        var producer = masstransit.Producer;
        cfg.Message<Mensagem>(configurator =>
        {
            configurator.SetEntityName(producer.ExchangeName);
        });
        cfg.Publish<Mensagem>(configurator =>
        {
            configurator.Durable = true;
            configurator.ExchangeType = ExchangeType.Topic;
        });

    });
});

//Add EF DbContext
builder.Services.AddDbContext<PessoaContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("PessoasDb"));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var pessoaContext = scope.ServiceProvider.GetRequiredService<PessoaContext>();
    pessoaContext.Database.EnsureCreated();
    pessoaContext.Seed();
}

app.UseMiddleware<OpenTelemetryMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
