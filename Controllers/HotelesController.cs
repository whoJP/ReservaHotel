using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservaHotel.Data;
using ReservaHotel.Models;

namespace ReservaHotel.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class HotelesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HotelesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Hoteles — Lista todos los hoteles
        public async Task<IActionResult> Index()
        {
            var hoteles = await _context.Hoteles.ToListAsync();
            return View(hoteles);
        }

        // GET: Hoteles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var hotel = await _context.Hoteles
                .Include(h => h.Reservas)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hotel == null) return NotFound();

            return View(hotel);
        }

        // GET: Hoteles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Hoteles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Direccion,PrecioNoche,Descripcion")] Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hotel);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Hotel creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(hotel);
        }

        // GET: Hoteles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var hotel = await _context.Hoteles.FindAsync(id);
            if (hotel == null) return NotFound();

            return View(hotel);
        }

        // POST: Hoteles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Direccion,PrecioNoche,Descripcion")] Hotel hotel)
        {
            if (id != hotel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hotel);
                    await _context.SaveChangesAsync();
                    TempData["Exito"] = "Hotel actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HotelExists(hotel.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(hotel);
        }

        // GET: Hoteles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var hotel = await _context.Hoteles
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hotel == null) return NotFound();

            return View(hotel);
        }

        // POST: Hoteles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hotel = await _context.Hoteles.FindAsync(id);
            if (hotel != null)
            {
                _context.Hoteles.Remove(hotel);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Hotel eliminado exitosamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool HotelExists(int id)
        {
            return _context.Hoteles.Any(e => e.Id == id);
        }
    }
}