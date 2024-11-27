using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TransportCompany.Models;

namespace TransportCompany.Data;

public partial class TransportCompanyContext : DbContext
{
    public TransportCompanyContext()
    {
    }

    public TransportCompanyContext(DbContextOptions<TransportCompanyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<CarBrand> CarBrands { get; set; }

    public virtual DbSet<CarType> CarTypes { get; set; }

    public virtual DbSet<Cargo> Cargos { get; set; }

    public virtual DbSet<CargoType> CargoTypes { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Trip> Trips { get; set; }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка связи между Car и Employee для Driver
        modelBuilder.Entity<Car>()
            .HasOne(c => c.Driver)
            .WithMany(e => e.CarDrivers)
            .HasForeignKey(c => c.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Настройка связи между Car и Employee для Mechanic
        modelBuilder.Entity<Car>()
            .HasOne(c => c.Mechanic)
            .WithMany(e => e.CarMechanics)
            .HasForeignKey(c => c.MechanicId)
            .OnDelete(DeleteBehavior.Restrict);

        OnModelCreatingPartial(modelBuilder);
    }


}
