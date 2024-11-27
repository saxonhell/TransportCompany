using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportCompany.Models;

public partial class Car
{
    public int Id { get; set; }

    public int? BrandId { get; set; }

    public int? CarTypeId { get; set; }

    public string RegistrationNumber { get; set; } = null!;

    public string? BodyNumber { get; set; }

    public string? EngineNumber { get; set; }

    public int? YearOfManufacture { get; set; }

    public int? DriverId { get; set; }

    public DateOnly? LastMaintenanceDate { get; set; }

    public int? MechanicId { get; set; }

    // Foreign Key Relations
    [ForeignKey("BrandId")]
    public virtual CarBrand? Brand { get; set; }

    [ForeignKey("CarTypeId")]
    public virtual CarType? CarType { get; set; }

    [ForeignKey("DriverId")]
    public virtual Employee? Driver { get; set; }

    [ForeignKey("MechanicId")]
    public virtual Employee? Mechanic { get; set; }

    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
