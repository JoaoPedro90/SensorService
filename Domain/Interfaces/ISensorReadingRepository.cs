using SensorService.Domain.Entities;

namespace SensorService.Domain.Interfaces;

public interface ISensorReadingRepository
{
    Task AddAsync(SensorReading reading, CancellationToken ct);
    Task<IReadOnlyList<SensorReading>> GetLatestByTalhaoAsync(Guid talhaoId, int limit, CancellationToken ct);
}