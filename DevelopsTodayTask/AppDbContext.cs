using DevelopsTodayTask.Models;
using System.Data.Entity;

public class AppDbContext : DbContext
{
    public AppDbContext() : base("name=ConnectionString") { }
    public DbSet<CabTripRecord> SampleCabData { get; set; }

    // Manually mapping table
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CabTripRecord>()
            .ToTable("Sample_Cab_Data")
            .Property(p => p.TpepPickupDatetime)
            .HasColumnName("tpep_pickup_datetime");

        modelBuilder.Entity<CabTripRecord>()
            .Property(p => p.TpepDropoffDatetime)
            .HasColumnName("tpep_dropoff_datetime");

        modelBuilder.Entity<CabTripRecord>()
            .Property(p => p.PassengerCount)
            .HasColumnName("passenger_count");

        modelBuilder.Entity<CabTripRecord>()
            .Property(p => p.TripDistance)
            .HasColumnName("trip_distance");

        modelBuilder.Entity<CabTripRecord>()
            .Property(p => p.StoreAndFwdFlag)
            .HasColumnName("store_and_fwd_flag");

        modelBuilder.Entity<CabTripRecord>()
            .Property(p => p.PULocationID)
            .HasColumnName("PULocationID");

        modelBuilder.Entity<CabTripRecord>()
            .Property(p => p.DOLocationID)
            .HasColumnName("DOLocationID");

        modelBuilder.Entity<CabTripRecord>()
            .Property(p => p.FareAmount)
            .HasColumnName("fare_amount");

        modelBuilder.Entity<CabTripRecord>()
            .Property(p => p.TipAmount)
            .HasColumnName("tip_amount");

        modelBuilder.Entity<CabTripRecord>()
            .HasKey(s => s.Id);
    }
}
