using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Events;

namespace SensorService.Infrastructure.Messaging;

/// <summary>
/// BackgroundService que fica rodando em paralelo ao ASP.NET, consumindo eventos do RabbitMQ.
/// Quando chega um TalhaoCreatedEvent, adiciona o TalhaoId no TalhaoRegistry.
/// </summary>
public sealed class TalhaoEventsConsumer : BackgroundService
{
    private readonly RabbitMqOptions _options;
    private readonly TalhaoRegistry _registry;
    private readonly ILogger<TalhaoEventsConsumer> _logger;

    private IConnection? _connection;
    private IModel? _channel;

    public TalhaoEventsConsumer(
        IOptions<RabbitMqOptions> options,
        TalhaoRegistry registry,
        ILogger<TalhaoEventsConsumer> logger)
    {
        _options = options.Value;
        _registry = registry;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Tenta para sempre (ou até cancelar)
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _options.Host,
                    Port = _options.Port,
                    UserName = _options.User,
                    Password = _options.Pass,
                    DispatchConsumersAsync = true,
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: _options.Exchange, type: ExchangeType.Topic, durable: true, autoDelete: false);
                _channel.QueueDeclare(queue: _options.Queue, durable: true, exclusive: false, autoDelete: false);
                _channel.QueueBind(queue: _options.Queue, exchange: _options.Exchange, routingKey: _options.RoutingKey);

                _channel.BasicQos(0, 1, false);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.Received += async (_, ea) =>
                {
                    try
                    {
                        var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                        var evt = JsonSerializer.Deserialize<TalhaoCreatedEvent>(json);

                        if (evt is null || evt.TalhaoId == Guid.Empty)
                        {
                            _logger.LogWarning("Mensagem inválida recebida na fila {Queue}: {Json}", _options.Queue, json);
                            _channel!.BasicAck(ea.DeliveryTag, multiple: false);
                            return;
                        }

                        _registry.Upsert(evt.TalhaoId, DateTime.UtcNow);
                        _logger.LogInformation("Talhão registrado via RabbitMQ: {TalhaoId}. Total conhecidos: {Count}", evt.TalhaoId, _registry.Count);

                        _channel!.BasicAck(ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao processar mensagem do RabbitMQ. Vai requeue=true para tentar novamente.");
                        _channel!.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
                    }

                    await Task.CompletedTask;
                };

                _channel.BasicConsume(queue: _options.Queue, autoAck: false, consumer: consumer);

                _logger.LogInformation("Consumidor RabbitMQ ativo. Exchange={Exchange}, Queue={Queue}, RoutingKey={Key}",
                    _options.Exchange, _options.Queue, _options.RoutingKey);

                // Mantém o BackgroundService vivo.
                // Se RabbitMQ cair e a conexão for quebrada, vamos sair do delay ao cancelar.
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Falha ao iniciar consumidor RabbitMQ (host={Host} port={Port}). Tentando novamente em 5s...",
                    _options.Host, _options.Port);

                // limpa recursos, para tentar de novo “do zero”
                try { _channel?.Close(); } catch { }
                try { _connection?.Close(); } catch { }
                _channel?.Dispose();
                _connection?.Dispose();
                _channel = null;
                _connection = null;

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        try { _channel?.Close(); } catch { }
        try { _connection?.Close(); } catch { }
        _channel?.Dispose();
        _connection?.Dispose();
        return base.StopAsync(cancellationToken);
    }
}
