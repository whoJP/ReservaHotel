using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservaHotel.Data;
using ReservaHotel.Models;

namespace ReservaHotel.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservasController(ApplicationDbContext context,
                                   UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservas — Lista las reservas del usuario actual
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            IQueryable<Reserva> reservas;

            if (User.IsInRole("Administrador"))
            {
                // Admin ve TODAS las reservas
                reservas = _context.Reservas
                    .Include(r => r.Hotel)
                    .Include(r => r.Usuario)
                    .OrderByDescending(r => r.FechaInicio);
            }
            else
            {
                // Cliente solo ve sus reservas
                reservas = _context.Reservas
                    .Include(r => r.Hotel)
                    .Include(r => r.Usuario)
                    .Where(r => r.UsuarioId == userId)
                    .OrderByDescending(r => r.FechaInicio);
            }

            return View(await reservas.ToListAsync());
        }

        // GET: Reservas/Create
        [Authorize(Roles = "Cliente")]
        public IActionResult Create()
        {
            ViewData["HotelId"] = new SelectList(_context.Hoteles, "Id", "Nombre");
            return View();
        }

        // POST: Reservas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Create([Bind("FechaInicio,FechaFin,HotelId")] Reserva reserva)
        {
            // Validación: fecha de fin mayor a fecha de inicio
            if (reserva.FechaFin <= reserva.FechaInicio)
            {
                ModelState.AddModelError("FechaFin",
                    "La fecha de fin debe ser posterior a la fecha de inicio.");
            }

            // Validación: no permitir reservas en fechas pasadas
            if (reserva.FechaInicio < DateTime.Today)
            {
                ModelState.AddModelError("FechaInicio",
                    "No puedes hacer reservas en fechas pasadas.");
            }

            if (ModelState.IsValid)
            {
                reserva.UsuarioId = _userManager.GetUserId(User)!;
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "¡Reserva realizada exitosamente!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["HotelId"] = new SelectList(_context.Hoteles, "Id", "Nombre", reserva.HotelId);
            return View(reserva);
        }

        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);

            var reserva = await _context.Reservas
                .Include(r => r.Hotel)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reserva == null) return NotFound();

            // Solo el dueño o el admin pueden ver/eliminar
            if (reserva.UsuarioId != userId && !User.IsInRole("Administrador"))
                return Forbid();

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var reserva = await _context.Reservas.FindAsync(id);

            if (reserva != null)
            {
                if (reserva.UsuarioId != userId && !User.IsInRole("Administrador"))
                    return Forbid();

                _context.Reservas.Remove(reserva);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Reserva cancelada exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}