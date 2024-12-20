﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransportCompany.Models
{
    public partial class Trip
    {
        public int Id { get; set; }

        public int? CarId { get; set; }

        [StringLength(200, ErrorMessage = "Имя заказчика не должно превышать 200 символов.")]
        public string? Customer { get; set; }

        [StringLength(100, ErrorMessage = "Местоположение отправления не должно превышать 100 символов.")]
        public string? Origin { get; set; }

        [StringLength(100, ErrorMessage = "Местоположение назначения не должно превышать 100 символов.")]
        public string? Destination { get; set; }

        public DateOnly? DepartureDate { get; set; }

        public DateOnly? ArrivalDate { get; set; }

        public int? CargoId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Цена должна быть положительным числом.")]
        public decimal? Price { get; set; }

        public bool? PaymentStatus { get; set; }

        public bool? ReturnStatus { get; set; }

        public int? DriverId { get; set; }

        [ForeignKey("CarId")]
        public virtual Car? Car { get; set; }

        [ForeignKey("CargoId")]
        public virtual Cargo? Cargo { get; set; }

        [ForeignKey("DriverId")]
        public virtual Employee? Driver { get; set; }
    }
}
