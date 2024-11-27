using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportCompany.Models;

public partial class CargoType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? CarTypeId { get; set; }

    public string? Description { get; set; }

    // Foreign Key Relation
    [ForeignKey("CarTypeId")]
    public virtual CarType? CarType { get; set; }

    public virtual ICollection<Cargo> Cargos { get; set; } = new List<Cargo>();
}
