namespace SensorService.Infrastructure.Messaging;

public sealed class RabbitMqOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string User { get; set; } = "guest";
    public string Pass { get; set; } = "guest";

    // De onde consumir
    public string Exchange { get; set; } = "property.events";
    public string Queue { get; set; } = "sensor.talhoes";
    public string RoutingKey { get; set; } = "talhao.created";
}
