using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportCompany.Models
{
    public partial class Cargo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Наименование груза обязательно для заполнения.")]
        [StringLength(200, ErrorMessage = "Наименование груза не должно превышать 200 символов.")]
        public string Name { get; set; } = null!;

        public int? CargoTypeId { get; set; }

        public DateOnly? ExpiryDate { get; set; }

        [StringLength(500, ErrorMessage = "Особенности не должны превышать 500 символов.")]
        public string? Features { get; set; }

        // Foreign Key Relation
        [ForeignKey("CargoTypeId")]
        public virtual CargoType? CargoType { get; set; }

        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
