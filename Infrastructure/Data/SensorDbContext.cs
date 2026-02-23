using Microsoft.EntityFrameworkCore;
using SensorService.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SensorService.Infrastructure.Data;

public class SensorDbContext : DbContext
{
    public SensorDbContext(DbContextOptions<SensorDbContext> options) : base(options) { }

    public DbSet<SensorReading> SensorReadings => Set<SensorReading>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SensorReading>(e =>
        {
            e.ToTable("SensorReadings");
            e.HasKey(x => x.Id);

            e.Property(x => x.SoilMoisture).HasColumnType("decimal(5,2)");
            e.Property(x => x.Temperature).HasColumnType("decimal(5,2)");
            e.Property(x => x.Precipitation).HasColumnType("decimal(8,2)");

            e.HasIndex(x => new { x.TalhaoId, x.TimestampUtc });
        });
    }
}