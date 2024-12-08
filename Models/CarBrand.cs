using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TransportCompany.Models
{
    public partial class CarBrand
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Наименование марки обязательно для заполнения.")]
        [StringLength(100, ErrorMessage = "Наименование марки не должно превышать 100 символов.")]
        public string Name { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Технические характеристики не должны превышать 500 символов.")]
        public string? TechnicalSpecifications { get; set; }

        [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов.")]
        public string? Description { get; set; }

        public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
    }
}
