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

namespace SchoolManagement.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Authorize(Roles = "Teacher")]
    public class DangKyLopKyNangsController : Controller
    {
        private readonly SchoolContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DangKyLopKyNangsController(SchoolContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Teacher/DangKyLopKyNangs
        public async Task<IActionResult> Index()
        {
            var currentUserId = GetUserId();
            var teacher = await _context.GiaoViens.FirstOrDefaultAsync(s => s.UserId == currentUserId);

            if (teacher != null)
            {
                var teacherSkills = await _context.DangKyLopKyNangs
                    .Include(dkkn => dkkn.MaLopKyNangNavigation)
                        .ThenInclude(lkn => lkn.MaBoMonNavigation)
                            .ThenInclude(bm => bm.Lops)
                                .ThenInclude(lp => lp.Phongs)
                    .Where(dkkn => dkkn.MaGiaoVien == teacher.MaGiaoVien)
                    .ToListAsync();

                return View(teacherSkills);
            }

            return RedirectToAction("Error");
        }





        [HttpGet]
        public IActionResult Create()
        {
            // Lấy danh sách các lớp kỹ năng từ cơ sở dữ liệu
            var lopKyNangs = _context.LopKyNangs.ToList();

            // Tạo SelectList với cả mã và tên kỹ năng
            var lopKyNangList = lopKyNangs.Select(lkn => new SelectListItem
            {
                Value = lkn.MaLopKyNang,
                Text = $"{lkn.MaLopKyNang} - {lkn.TenLopKyNang}" // Hiển thị mã và tên kỹ năng
            }).ToList();

            // Truyền SelectList vào ViewBag hoặc ViewData để sử dụng trong View
            ViewBag.MaLopKyNang = new SelectList(lopKyNangList, "Value", "Text");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaGiaoVien, MaLopKyNang")] DangKyLopKyNang dangKyLopKyNang)
        {
            int currentUserId = GetUserId();

            // Lấy thông tin giáo viên dựa trên UserID
            var teacher = _context.GiaoViens.FirstOrDefault(s => s.UserId == currentUserId);

            if (teacher != null)
            {
                dangKyLopKyNang.MaGiaoVien = teacher.MaGiaoVien; // Gán MaGiaoVien với giáo viên đăng nhập
            }
            else
            {
                // Xử lý khi không tìm thấy thông tin giáo viên
                // Ví dụ: Redirect hoặc thông báo lỗi
                return RedirectToAction("Error");
            }
            // Kiểm tra xem giáo viên đã đăng ký lớp môn học này chưa
            var existingRegistration = _context.DangKyLopKyNangs
                .FirstOrDefault(r => r.MaGiaoVien == dangKyLopKyNang.MaGiaoVien && r.MaLopKyNang == dangKyLopKyNang.MaLopKyNang);

            if (existingRegistration != null)
            {
                ModelState.AddModelError("MaLopKyNang", "Giáo viên đã đăng ký lớp môn học này");
            }
            if (ModelState.IsValid)
            {
                // Tìm bản ghi có id lớn nhất trong bảng DangKyLopKyNangs
                var maxId = _context.DangKyLopKyNangs.Max(x => x.Id);

                dangKyLopKyNang.Id = maxId + 1;

                _context.Add(dangKyLopKyNang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaLopKyNang"] = new SelectList(_context.LopKyNangs, "MaLopKyNang", "MaLopKyNang", dangKyLopKyNang.MaLopKyNang);
            return View(dangKyLopKyNang);
        }

        private int GetUserId()
        {
            if (_httpContextAccessor.HttpContext.User.Identity is ClaimsIdentity identity && identity.IsAuthenticated)
            {

                var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }

            return 0;
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DangKyLopKyNangs == null)
            {
                return NotFound();
            }

            var dangKyLopKyNang = await _context.DangKyLopKyNangs
                .Include(d => d.MaGiaoVienNavigation)
                .Include(d => d.MaLopKyNangNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dangKyLopKyNang == null)
            {
                return NotFound();
            }

            return View(dangKyLopKyNang);
        }

        // POST: Teacher/DangKyLopKyNangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DangKyLopKyNangs == null)
            {
                return Problem("Entity set 'SchoolContext.DangKyLopKyNangs'  is null.");
            }
            var dangKyLopKyNang = await _context.DangKyLopKyNangs.FindAsync(id);
            if (dangKyLopKyNang != null)
            {
                _context.DangKyLopKyNangs.Remove(dangKyLopKyNang);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DangKyLopKyNangExists(int id)
        {
          return (_context.DangKyLopKyNangs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
