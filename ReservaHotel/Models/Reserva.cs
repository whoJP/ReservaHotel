using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservaHotel.Models
{
    public class Reserva : IValidatableObject
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Fin")]
        public DateTime FechaFin { get; set; }

        // Relación con Hotel
        [Required]
        public int HotelId { get; set; }
        [ForeignKey("HotelId")]
        public virtual Hotel? Hotel { get; set; }

        // Relación con Usuario (Identity)
        [Required]
        public string UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public virtual ApplicationUser? Usuario { get; set; }

        // Lógica de validación personalizada (El "secreto" de la receta)
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaInicio < DateTime.Today)
            {
                yield return new ValidationResult("No puedes reservar en fechas pasadas.", new[] { nameof(FechaInicio) });
            }

            if (FechaFin <= FechaInicio)
            {
                yield return new ValidationResult("La fecha de fin debe ser posterior a la de inicio.", new[] { nameof(FechaFin) });
            }
        }
    }
}