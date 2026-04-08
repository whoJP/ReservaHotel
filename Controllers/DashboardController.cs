using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservaHotel.Data;
using ReservaHotel.Models;

namespace ReservaHotel.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context,
                                    UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                TotalHoteles = await _context.Hoteles.CountAsync(),
                TotalUsuarios = _userManager.Users.Count(),
                TotalReservas = await _context.Reservas.CountAsync(),

                ReservasPorMes = await _context.Reservas
                    .Where(r => r.FechaInicio.Year == DateTime.Now.Year)
                    .GroupBy(r => r.FechaInicio.Month)
                    .Select(g => new ReservasPorMes
                    {
                        Mes = g.Key.ToString(),
                        Cantidad = g.Count()
                    })
                    .OrderBy(x => x.Mes)
                    .ToListAsync(),

                HotelesMasReservados = await _context.Reservas
                    .Include(r => r.Hotel)
                    .GroupBy(r => r.Hotel!.Nombre)
                    .Select(g => new HotelMasReservado
                    {
                        NombreHotel = g.Key,
                        CantidadReservas = g.Count()
                    })
                    .OrderByDescending(x => x.CantidadReservas)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }
}