using ExampleOtelLibrary;
using MassTransit;
using RabbitMQ.Client;
using WebAPI;
using Worker;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        //services.AddHostedService<WorkerService>();
    });

builder.ConfigureAppConfiguration(configBuilder =>
{
    var configuration = configBuilder.Build();
    var masstransit = configuration.GetRequiredSection(nameof(MasstransitConfiguration)).Get<MasstransitConfiguration>();

    builder.ConfigureLogging(logging =>
    {
        logging.AddExampleLogging(configuration);
    });
    builder.ConfigureServices(services =>
    {
        services.AddExampleMetricsAndTracingOpenTelemetry(configuration);

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var broker = masstransit.Broker;
                cfg.Host(broker.Host, broker.Port, broker.VirtualHost, c =>
                {
                    c.Username(broker.Username);
                    c.Password(broker.Password);
                });

                //Consumer configuration
                var consumer = masstransit.Consumer;
                cfg.ReceiveEndpoint(consumer.QueueName, e =>
                {
                    e.ConfigureConsumeTopology = false;
                    e.PrefetchCount = consumer.PrefetchCount;
                    e.ConcurrentMessageLimit = consumer.ConcurrencyLimit;
                    e.Bind(consumer.ExchangeName, configurator =>
                    {
                        configurator.Durable = true;
                        configurator.ExchangeType = ExchangeType.Topic;
                    });

                    e.DiscardSkippedMessages();
                    e.DiscardFaultedMessages();
                    e.Consumer<MensagemConsumer>(context);
                });

                var producer = masstransit.Producer;
                cfg.Message<Mensagem>(configurator =>
                {
                    configurator.SetEntityName(producer.ExchangeName);
                });


            });
            x.AddConsumer<MensagemConsumer>();
        });
    });

});

IHost host = builder.Build();

await host.RunAsync();
