using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservaHotel.Data;
using ReservaHotel.Models;
using ReservaHotel.ViewModels;
using Rotativa.AspNetCore;
using System.Security.Claims;

namespace ReservaHotel.Controllers
{
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Dashboard del Administrador
        public async Task<IActionResult> Dashboard()
        {
            var modelo = new DashboardVM
            {
                TotalHoteles = await _context.Hoteles.CountAsync(),
                TotalUsuarios = await _context.Users.CountAsync(),
                TotalReservas = await _context.Reservas.CountAsync(),
                ReservasPorMes = new List<int> { 5, 10, 15, 7, 20 } // Datos de ejemplo para la gráfica
            };
            return View(modelo);
        }

        // 2. Crear Reserva (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reservar(int hotelId, DateTime inicio, DateTime fin)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Obtiene el ID del usuario logueado

            var reserva = new Reserva
            {
                HotelId = hotelId,
                UsuarioId = userId,
                FechaInicio = inicio,
                FechaFin = fin
            };

            if (ModelState.IsValid)
            {
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction("MisReservas");
            }
            return BadRequest("Datos inválidos");
        }
        public async Task<IActionResult> DescargarPDF()
        {
            var reservas = await _context.Reservas.Include(r => r.Hotel).Include(r => r.Usuario).ToListAsync();
            return new ViewAsPdf("ListaReservasPDF", reservas)
            {
                FileName = "Reporte_Reservas.pdf"
            };
        }
    }
}