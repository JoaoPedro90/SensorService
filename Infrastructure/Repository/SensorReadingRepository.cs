using Microsoft.EntityFrameworkCore;
using SensorService.Domain.Entities;
using SensorService.Domain.Interfaces;
using SensorService.Infrastructure.Data;

namespace SensorService.Infrastructure.Repositories;

public class SensorReadingRepository : ISensorReadingRepository
{
    private readonly SensorDbContext _db;

    public SensorReadingRepository(SensorDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(SensorReading reading, CancellationToken ct)
    {
        _db.SensorReadings.Add(reading);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<SensorReading>> GetLatestByTalhaoAsync(Guid talhaoId, int limit, CancellationToken ct)
    {
        limit = Math.Clamp(limit, 1, 500);

        return await _db.SensorReadings
            .Where(x => x.TalhaoId == talhaoId)
            .OrderByDescending(x => x.TimestampUtc)
            .Take(limit)
            .AsNoTracking()
            .ToListAsync(ct);
    }
}