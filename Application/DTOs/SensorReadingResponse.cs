namespace SensorService.Application.Dtos;

public class SensorReadingResponse
{
    public Guid Id { get; set; }
    public Guid TalhaoId { get; set; }
    public decimal SoilMoisture { get; set; }
    public decimal Temperature { get; set; }
    public decimal Precipitation { get; set; }
    public DateTime TimestampUtc { get; set; }
}