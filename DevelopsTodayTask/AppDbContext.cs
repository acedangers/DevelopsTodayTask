using DevelopsTodayTask.Models;
using System.Data.Entity;

public class AppDbContext : DbContext
{
    public DbSet<CabTripRecord> Trips { get; set; }

    public AppDbContext() : base("name=ConnectionString")
    {
    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CabTripRecord>().ToTable("Sample_Cab_Data");
        modelBuilder.Entity<CabTripRecord>().HasKey(t => new { t.TpepPickupDatetime, t.TpepDropoffDatetime });
    }
}
