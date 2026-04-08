using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ReservaHotel.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        // Relación: un usuario puede tener muchas reservas
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}