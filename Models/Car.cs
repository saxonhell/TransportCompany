using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportCompany.Models
{
    public partial class Car
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Марка автомобиля обязательна для заполнения.")]
        public int? BrandId { get; set; }

        [Required(ErrorMessage = "Тип автомобиля обязателен для заполнения.")]
        public int? CarTypeId { get; set; }

        [Required(ErrorMessage = "Регистрационный номер обязателен для заполнения.")]
        [StringLength(20, ErrorMessage = "Регистрационный номер не должен превышать 20 символов.")]
        public string RegistrationNumber { get; set; } = null!;

        [StringLength(50, ErrorMessage = "Номер кузова не должен превышать 50 символов.")]
        public string? BodyNumber { get; set; }

        [StringLength(50, ErrorMessage = "Номер двигателя не должен превышать 50 символов.")]
        public string? EngineNumber { get; set; }

        [Range(1900, 2100, ErrorMessage = "Год выпуска должен быть в диапазоне от 1900 до 2100.")]
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
}
