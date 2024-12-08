using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportCompany.Models
{
    public partial class CargoType
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Наименование типа груза обязательно для заполнения.")]
        [StringLength(100, ErrorMessage = "Наименование типа груза не должно превышать 100 символов.")]
        public string Name { get; set; } = null!;

        public int? CarTypeId { get; set; }

        [StringLength(200, ErrorMessage = "Описание не должно превышать 200 символов.")]
        public string? Description { get; set; }

        // Foreign Key Relation
        [ForeignKey("CarTypeId")]
        public virtual CarType? CarType { get; set; }

        public virtual ICollection<Cargo> Cargos { get; set; } = new List<Cargo>();
    }
}
