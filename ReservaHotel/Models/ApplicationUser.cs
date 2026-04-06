using Microsoft.AspNetCore.Identity;

namespace ReservaHotel.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string NombreCompleto { get; set; }
    }
}