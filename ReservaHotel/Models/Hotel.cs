using System.ComponentModel.DataAnnotations;

namespace ReservaHotel.Models
{
    public class Hotel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Direccion { get; set; }

        [Required]
        [Range(1, 10000)]
        public decimal PrecioPorNoche { get; set; }

        public string? Descripcion { get; set; }

        // Relación inversa: Un hotel tiene muchas reservas
        public virtual ICollection<Reserva>? Reservas { get; set; }
    }
}