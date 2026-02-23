namespace SensorService.Application.Interfaces;

public interface ITalhaoValidator
{
    Task<bool> ExistsAsync(Guid talhaoId, CancellationToken ct);
}