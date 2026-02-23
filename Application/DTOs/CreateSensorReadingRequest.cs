namespace SensorService.Application.Dtos;

public class CreateSensorReadingRequest
{
    public decimal SoilMoisture { get; set; }
    public decimal Temperature { get; set; }
    public decimal Precipitation { get; set; }

    // Opcional: se não vier, usa "agora"
    public DateTime? TimestampUtc { get; set; }
}