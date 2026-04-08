using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReservaHotel.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Fin")]
        public DateTime FechaFin { get; set; }

        // Clave foránea al usuario
        [Required]
        public string UsuarioId { get; set; } = string.Empty;

        // Propiedad de navegación al usuario
        [ForeignKey("UsuarioId")]
        public ApplicationUser? Usuario { get; set; }

        // Clave foránea al hotel
        [Required(ErrorMessage = "Debes seleccionar un hotel")]
        [Display(Name = "Hotel")]
        public int HotelId { get; set; }

        // Propiedad de navegación al hotel
        [ForeignKey("HotelId")]
        public Hotel? Hotel { get; set; }

        // Propiedad calculada (no se almacena en BD)
        [NotMapped]
        public int TotalNoches => (FechaFin - FechaInicio).Days;

        [NotMapped]
        public decimal TotalPagar => TotalNoches * (Hotel?.PrecioNoche ?? 0);
    }
}