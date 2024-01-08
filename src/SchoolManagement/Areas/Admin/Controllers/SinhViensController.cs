using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Models;

namespace SchoolManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SinhViensController : Controller
    {
        private readonly SchoolContext _context;

        public SinhViensController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Admin/SinhViens
        public async Task<IActionResult> Index()
        {
            var schoolContext = _context.SinhViens.Include(s => s.MaLopNavigation).Include(s => s.User);
            return View(await schoolContext.ToListAsync());
        }

        // GET: Admin/SinhViens/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.SinhViens == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.SinhViens
                .Include(s => s.MaLopNavigation)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.MaSinhVien == id);
            if (sinhVien == null)
            {
                return NotFound();
            }

            return View(sinhVien);
        }

        // GET: Admin/SinhViens/Create
        public IActionResult Create()
        {
            ViewData["MaLop"] = new SelectList(_context.Lops, "MaLop", "MaLop");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Admin/SinhViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSinhVien,UserId,Ten,NgaySinh,GioiTinh,MaLop,DanToc")] SinhVien sinhVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sinhVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaLop"] = new SelectList(_context.Lops, "MaLop", "MaLop", sinhVien.MaLop);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", sinhVien.UserId);
            return View(sinhVien);
        }

        // GET: Admin/SinhViens/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.SinhViens == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien == null)
            {
                return NotFound();
            }
            ViewData["MaLop"] = new SelectList(_context.Lops, "MaLop", "MaLop", sinhVien.MaLop);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", sinhVien.UserId);
            return View(sinhVien);
        }

        // POST: Admin/SinhViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaSinhVien,UserId,Ten,NgaySinh,GioiTinh,MaLop,DanToc")] SinhVien sinhVien)
        {
            if (id != sinhVien.MaSinhVien)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sinhVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SinhVienExists(sinhVien.MaSinhVien))
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
            ViewData["MaLop"] = new SelectList(_context.Lops, "MaLop", "MaLop", sinhVien.MaLop);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", sinhVien.UserId);
            return View(sinhVien);
        }

        // GET: Admin/SinhViens/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.SinhViens == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.SinhViens
                .Include(s => s.MaLopNavigation)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.MaSinhVien == id);
            if (sinhVien == null)
            {
                return NotFound();
            }

            return View(sinhVien);
        }

        // POST: Admin/SinhViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.SinhViens == null)
            {
                return Problem("Entity set 'SchoolContext.SinhViens'  is null.");
            }
            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien != null)
            {
                _context.SinhViens.Remove(sinhVien);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SinhVienExists(string id)
        {
          return (_context.SinhViens?.Any(e => e.MaSinhVien == id)).GetValueOrDefault();
        }
    }
}
