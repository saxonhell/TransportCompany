using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportCompany.Models;

public partial class Cargo
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? CargoTypeId { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public string? Features { get; set; }

    // Foreign Key Relation
    [ForeignKey("CargoTypeId")]
    public virtual CargoType? CargoType { get; set; }

    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
