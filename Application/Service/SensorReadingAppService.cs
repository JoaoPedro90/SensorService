using SensorService.Application.Dtos;
using SensorService.Application.Interfaces;
using SensorService.Domain.Entities;
using SensorService.Domain.Interfaces;

namespace SensorService.Application.Services;

public class SensorReadingAppService
{
    private readonly ISensorReadingRepository _repo;
    private readonly ITalhaoValidator _talhaoValidator;

    public SensorReadingAppService(ISensorReadingRepository repo, ITalhaoValidator talhaoValidator)
    {
        _repo = repo;
        _talhaoValidator = talhaoValidator;
    }

    public async Task<SensorReadingResponse> IngestAsync(Guid talhaoId, CreateSensorReadingRequest req, CancellationToken ct)
    {
        // valida se talhão existe (simulado ou integração real)
        var exists = await _talhaoValidator.ExistsAsync(talhaoId, ct);
        if (!exists)
            throw new InvalidOperationException("Talhão não encontrado (ou não autorizado).");

        var timestamp = req.TimestampUtc ?? DateTime.UtcNow;

        var entity = new SensorReading(
            talhaoId,
            req.SoilMoisture,
            req.Temperature,
            req.Precipitation,
            timestamp
        );

        await _repo.AddAsync(entity, ct);

        return new SensorReadingResponse
        {
            Id = entity.Id,
            TalhaoId = entity.TalhaoId,
            SoilMoisture = entity.SoilMoisture,
            Temperature = entity.Temperature,
            Precipitation = entity.Precipitation,
            TimestampUtc = entity.TimestampUtc
        };
    }

    public async Task<IReadOnlyList<SensorReadingResponse>> GetLatestAsync(Guid talhaoId, int limit, CancellationToken ct)
    {
        var list = await _repo.GetLatestByTalhaoAsync(talhaoId, limit, ct);

        return list.Select(x => new SensorReadingResponse
        {
            Id = x.Id,
            TalhaoId = x.TalhaoId,
            SoilMoisture = x.SoilMoisture,
            Temperature = x.Temperature,
            Precipitation = x.Precipitation,
            TimestampUtc = x.TimestampUtc
        }).ToList();
    }
}