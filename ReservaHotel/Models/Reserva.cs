using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReservaHotel.Models
{
    public class Reserva : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [Required]
        public int HotelId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaFin <= FechaInicio)
            {
                yield return new ValidationResult(
                    "La fecha de fin debe ser mayor que la fecha de inicio",
                    new[] { nameof(FechaFin) }
                );
            }

            if (FechaInicio < DateTime.Today)
            {
                yield return new ValidationResult(
                    "No puedes reservar en fechas pasadas",
                    new[] { nameof(FechaInicio) }
                );
            }
        }
    }
} 