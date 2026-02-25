using SensorService.Application.Interfaces;
using SensorService.Infrastructure.Messaging;

namespace SensorService.Infrastructure.Integrations;

/// <summary>
/// Valida talhões usando a lista que chega via RabbitMQ.
/// </summary>
public sealed class RabbitMqTalhaoValidator : ITalhaoValidator
{
    private readonly TalhaoRegistry _registry;

    public RabbitMqTalhaoValidator(TalhaoRegistry registry)
        => _registry = registry;

    public Task<bool> ExistsAsync(Guid talhaoId, CancellationToken ct)
        => Task.FromResult(talhaoId != Guid.Empty && _registry.Contains(talhaoId));
}
