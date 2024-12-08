using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TransportCompany.Models
{
    public partial class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Имя сотрудника обязательно для заполнения.")]
        [StringLength(100, ErrorMessage = "Имя сотрудника не должно превышать 100 символов.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Роль сотрудника обязательна для заполнения.")]
        [StringLength(100, ErrorMessage = "Роль сотрудника не должна превышать 100 символов.")]
        public string Role { get; set; } = null!;

        public virtual ICollection<Car> CarDrivers { get; set; } = new List<Car>();

        public virtual ICollection<Car> CarMechanics { get; set; } = new List<Car>();

        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
