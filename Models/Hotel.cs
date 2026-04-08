using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservaHotel.Models
{
    public class Hotel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del hotel es obligatorio")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres")]
        [Display(Name = "Nombre del Hotel")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(250, ErrorMessage = "La dirección no puede exceder 250 caracteres")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio por noche es obligatorio")]
        [Range(1, 99999, ErrorMessage = "El precio debe ser entre 1 y 99,999")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Precio por Noche ($)")]
        public decimal PrecioNoche { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        // Relación: un hotel puede tener muchas reservas
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}