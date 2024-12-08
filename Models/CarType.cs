using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TransportCompany.Models
{
    public partial class CarType
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Наименование типа автомобиля обязательно для заполнения.")]
        [StringLength(100, ErrorMessage = "Наименование типа автомобиля не должно превышать 100 символов.")]
        public string Name { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов.")]
        public string? Description { get; set; }

        public virtual ICollection<CargoType> CargoTypes { get; set; } = new List<CargoType>();

        public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
    }
}
