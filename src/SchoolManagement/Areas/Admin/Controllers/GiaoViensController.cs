using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagement.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace SchoolManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class GiaoViensController : Controller
    {
        private readonly SchoolContext _context;

        public GiaoViensController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Admin/GiaoViens
        public async Task<IActionResult> Index()
        {
            var schoolContext = _context.GiaoViens.Include(g => g.MaBoMonNavigation).Include(g => g.User);
            return View(await schoolContext.ToListAsync());
        }

        // GET: Admin/GiaoViens/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.GiaoViens == null)
            {
                return NotFound();
            }

            var giaoVien = await _context.GiaoViens
                .Include(g => g.MaBoMonNavigation)
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.MaGiaoVien == id);
            if (giaoVien == null)
            {
                return NotFound();
            }

            return View(giaoVien);
        }

        // GET: Admin/GiaoViens/Create
        public IActionResult Create()
        {
            ViewData["MaBoMon"] = new SelectList(_context.BoMons, "MaBoMon", "MaBoMon");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Admin/GiaoViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaGiaoVien,UserId,TenGiaoVien,NgaySinh,GioiTinh,MaBoMon")] GiaoVien giaoVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(giaoVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaBoMon"] = new SelectList(_context.BoMons, "MaBoMon", "MaBoMon", giaoVien.MaBoMon);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", giaoVien.UserId);
            return View(giaoVien);
        }

        // GET: Admin/GiaoViens/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.GiaoViens == null)
            {
                return NotFound();
            }

            var giaoVien = await _context.GiaoViens.FindAsync(id);
            if (giaoVien == null)
            {
                return NotFound();
            }
            ViewData["MaBoMon"] = new SelectList(_context.BoMons, "MaBoMon", "MaBoMon", giaoVien.MaBoMon);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", giaoVien.UserId);
            return View(giaoVien);
        }

        // POST: Admin/GiaoViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaGiaoVien,UserId,TenGiaoVien,NgaySinh,GioiTinh,MaBoMon")] GiaoVien giaoVien)
        {
            if (id != giaoVien.MaGiaoVien)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(giaoVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiaoVienExists(giaoVien.MaGiaoVien))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaBoMon"] = new SelectList(_context.BoMons, "MaBoMon", "MaBoMon", giaoVien.MaBoMon);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", giaoVien.UserId);
            return View(giaoVien);
        }

        // GET: Admin/GiaoViens/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.GiaoViens == null)
            {
                return NotFound();
            }

            var giaoVien = await _context.GiaoViens
                .Include(g => g.MaBoMonNavigation)
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.MaGiaoVien == id);
            if (giaoVien == null)
            {
                return NotFound();
            }

            return View(giaoVien);
        }

        // POST: Admin/GiaoViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.GiaoViens == null)
            {
                return Problem("Entity set 'SchoolContext.GiaoViens'  is null.");
            }
            var giaoVien = await _context.GiaoViens.FindAsync(id);
            if (giaoVien != null)
            {
                _context.GiaoViens.Remove(giaoVien);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GiaoVienExists(string id)
        {
          return (_context.GiaoViens?.Any(e => e.MaGiaoVien == id)).GetValueOrDefault();
        }
    }
}
