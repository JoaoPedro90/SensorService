using SensorService.Application.Interfaces;

namespace SensorService.Infrastructure.Integrations;

public class DummyTalhaoValidator : ITalhaoValidator
{
    // Para começar: aceita qualquer GUID não-vazio como "talhão existente"
    public Task<bool> ExistsAsync(Guid talhaoId, CancellationToken ct)
        => Task.FromResult(talhaoId != Guid.Empty);
}