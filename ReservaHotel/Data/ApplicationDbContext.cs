using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReservaHotel.Models;

namespace ReservaHotel.Data
{
    // Usaremos el formato tradicional para que la herramienta de migración no se pierda
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Hotel> Hoteles { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
    }
}