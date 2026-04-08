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

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            IQueryable<Reserva> reservas;

            if (User.IsInRole("Administrador"))
            {
                reservas = _context.Reservas
                    .Include(r => r.Hotel)
                    .Include(r => r.Usuario)
                    .OrderByDescending(r => r.FechaInicio);
            }
            else
            {
                reservas = _context.Reservas
                    .Include(r => r.Hotel)
                    .Include(r => r.Usuario)
                    .Where(r => r.UsuarioId == userId)
                    .OrderByDescending(r => r.FechaInicio);
            }

            return View(await reservas.ToListAsync());
        }

        [Authorize(Roles = "Cliente")]
        public IActionResult Create()
        {
            ViewData["HotelId"] = new SelectList(_context.Hoteles, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Create([Bind("FechaInicio,FechaFin,HotelId")] Reserva reserva)
        {
            // Quitar propiedades de navegación del ModelState para que no interfieran
            ModelState.Remove("Usuario");
            ModelState.Remove("UsuarioId");
            ModelState.Remove("Hotel");

            // ── Validaciones de fechas ──────────────────────────────

            // Fechas nulas
            if (reserva.FechaInicio == default)
            {
                ModelState.AddModelError("FechaInicio", "La fecha de inicio es obligatoria.");
            }

            if (reserva.FechaFin == default)
            {
                ModelState.AddModelError("FechaFin", "La fecha de fin es obligatoria.");
            }

            if (reserva.FechaInicio != default && reserva.FechaFin != default)
            {
                // Fecha inicio no puede ser pasada
                if (reserva.FechaInicio < DateTime.Today)
                {
                    ModelState.AddModelError("FechaInicio",
                        "La fecha de inicio no puede ser una fecha pasada.");
                }

                // Fecha inicio no mayor a 1 año desde hoy
                if (reserva.FechaInicio > DateTime.Today.AddYears(1))
                {
                    ModelState.AddModelError("FechaInicio",
                        "La fecha de inicio no puede ser mayor a 1 año desde hoy.");
                }

                // Fecha fin debe ser estrictamente mayor a fecha inicio (mínimo 1 noche)
                if (reserva.FechaFin <= reserva.FechaInicio)
                {
                    ModelState.AddModelError("FechaFin",
                        "La fecha de fin debe ser posterior a la fecha de inicio (mínimo 1 noche).");
                }

                // Máximo 30 días de reserva
                if (reserva.FechaFin > reserva.FechaInicio &&
                    (reserva.FechaFin - reserva.FechaInicio).Days > 30)
                {
                    ModelState.AddModelError("FechaFin",
                        "La reserva no puede superar los 30 días.");
                }
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

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);

            var reserva = await _context.Reservas
                .Include(r => r.Hotel)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reserva == null) return NotFound();

            if (reserva.UsuarioId != userId && !User.IsInRole("Administrador"))
                return Forbid();

            return View(reserva);
        }

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