using SensorService.Domain.Exceptions;

namespace SensorService.Domain.Entities;

public class SensorReading
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid TalhaoId { get; private set; }

    public decimal SoilMoisture { get; private set; }      // 0..100 (%)
    public decimal Temperature { get; private set; }       // °C
    public decimal Precipitation { get; private set; }     // mm
    public DateTime TimestampUtc { get; private set; }     // sempre UTC

    // EF
    private SensorReading() { }

    public SensorReading(Guid talhaoId, decimal soilMoisture, decimal temperature, decimal precipitation, DateTime timestampUtc)
    {
        if (talhaoId == Guid.Empty) throw new DomainException("TalhaoId inválido.");

        if (soilMoisture < 0 || soilMoisture > 100)
            throw new DomainException("Umidade do solo deve estar entre 0 e 100.");

        if (precipitation < 0)
            throw new DomainException("Precipitação não pode ser negativa.");

        // Ajuste se quiser uma faixa diferente
        if (temperature < -50 || temperature > 80)
            throw new DomainException("Temperatura fora da faixa permitida (-50 a 80°C).");

        TalhaoId = talhaoId;
        SoilMoisture = soilMoisture;
        Temperature = temperature;
        Precipitation = precipitation;
        TimestampUtc = DateTime.SpecifyKind(timestampUtc, DateTimeKind.Utc);
    }
}