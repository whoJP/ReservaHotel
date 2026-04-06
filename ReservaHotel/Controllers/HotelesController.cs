using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservaHotel.Data;
using ReservaHotel.Models;
using Microsoft.AspNetCore.Authorization;

namespace ReservaHotel.Controllers
{
    // Solo los Administradores pueden gestionar hoteles (según el proyecto)
    [Authorize(Roles = "Administrador")]
    public class HotelesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HotelesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Hoteles (Listado)
        [AllowAnonymous] // Permitir que clientes vean la lista
        public async Task<IActionResult> Index()
        {
            return View(await _context.Hoteles.ToListAsync());
        }

        // GET: Hoteles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Hoteles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hotel);
                await _context.SaveChangesAsync();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Hotel hotel)
        {
            if (id != hotel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(hotel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hotel);
        }

        // GET: Hoteles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var hotel = await _context.Hoteles.FirstOrDefaultAsync(m => m.Id == id);
            if (hotel == null) return NotFound();
            return View(hotel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hotel = await _context.Hoteles.FindAsync(id);
            if (hotel != null) _context.Hoteles.Remove(hotel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}