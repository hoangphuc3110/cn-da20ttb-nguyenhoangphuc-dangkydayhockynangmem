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
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace SchoolManagement.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Authorize(Roles = "Teacher")]
    public class ChamDiemsController : Controller
    {
        private readonly SchoolContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChamDiemsController(SchoolContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Print()
        {
            int userId = GetUserId(); // Lấy ID của giảng viên đăng nhập

            // Tìm giảng viên dựa trên ID của người dùng
            var teacher = _context.GiaoViens.FirstOrDefault(s => s.UserId == userId);

            if (teacher != null)
            {
                // Lấy danh sách sinh viên từ danh sách kỹ năng đã đăng ký của giảng viên
                var registeredStudents = await _context.DangKyLopKyNangs
                    .Include(dkln => dkln.MaLopKyNangNavigation)
                        .ThenInclude(ln => ln.KyNangs)
                            .ThenInclude(kn => kn.DangKyKyNangs)
                                .ThenInclude(dkkn => dkkn.MaSinhVienNavigation)
                    .Where(dkln => dkln.MaGiaoVien == teacher.MaGiaoVien)
                    .SelectMany(dkln => dkln.MaLopKyNangNavigation.KyNangs.SelectMany(kn => kn.DangKyKyNangs.Select(dkkn => dkkn.MaSinhVienNavigation)))
                    .Distinct()
                    .ToListAsync();

                // Create Excel package
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("SinhVienList");

                    // Set headers
                    worksheet.Cells[1, 1].Value = "Mã Sinh Viên";
                    worksheet.Cells[1, 2].Value = "Họ Tên";
                    worksheet.Cells[1, 3].Value = "Mã lớp";

                    // Populate data
                    int row = 2;
                    foreach (var student in registeredStudents)
                    {
                        worksheet.Cells[row, 1].Value = student.MaSinhVien;
                        worksheet.Cells[row, 2].Value = student.Ten;
                        worksheet.Cells[row, 3].Value = student.MaLop;
                        row++;
                    }

                    // Auto-fit columns for better visibility
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Set the content type and return the file
                    var content = package.GetAsByteArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SinhVienList.xlsx");
                }
            }

            // Xử lý khi không tìm thấy thông tin giáo viên
            // Ví dụ: Redirect hoặc thông báo lỗi
            return RedirectToAction("Error");
        }
        // GET: Teacher/ChamDiems
        public async Task<IActionResult> Index()
        {
            // Lấy ID của giáo viên hiện tại từ hệ thống đăng nhập của bạn
            var currentUserId = GetUserId();

            // Lấy thông tin của giáo viên dựa trên ID
            var teacher = await _context.GiaoViens.FirstOrDefaultAsync(s => s.UserId == currentUserId);

            if (teacher != null)
            {
                // Lấy danh sách điểm của giáo viên từ database
                var teacherGrades = await _context.ChamDiems
                    .Include(cd => cd.MaSinhVienNavigation)
                    .Include(cd => cd.MaKyNangNavigation)
                    .Where(cd => cd.MaGiaoVien == teacher.MaGiaoVien)
                    .ToListAsync();

                return View(teacherGrades);
            }

            // Xử lý khi không tìm thấy thông tin giáo viên
            // Ví dụ: Redirect hoặc thông báo lỗi
            return RedirectToAction("Error");
        }
        public async Task<IActionResult> List()
        {
            int userId = GetUserId(); // Lấy ID của giảng viên đăng nhập

            var registeredStudents = await _context.GiaoViens
                .Where(gv => gv.UserId == userId)
                .SelectMany(gv => gv.DangKyLopKyNangs.SelectMany(dkln => dkln.MaLopKyNangNavigation.KyNangs.SelectMany(kn => kn.DangKyKyNangs.Select(dkkn => dkkn.MaSinhVienNavigation))))
                .Distinct()
                .ToListAsync();

            return View(registeredStudents);
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


        // GET: Teacher/ChamDiems/Create
        public IActionResult Create()
        {
            var currentUserId = GetUserId(); // Lấy ID của giáo viên đăng nhập

            // Khởi tạo một đối tượng ChamDiem và gán MaGiaoVien cho giáo viên đang đăng nhập
            var chamDiem = new ChamDiem
            {
                MaGiaoVien = currentUserId.ToString() // Gán MaGiaoVien cho giáo viên đăng nhập
            };

            // Tạo SelectList cho MaKyNang và MaSinhVien tương tự như trước
            ViewData["MaKyNang"] = new SelectList(_context.KyNangs, "MaKyNang", "MaKyNang");
            ViewData["MaSinhVien"] = new SelectList(_context.SinhViens, "MaSinhVien", "MaSinhVien");

            // Truyền chamDiem đã tạo và SelectList vào View
            return View(chamDiem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MaGiaoVien,MaSinhVien,MaKyNang,Dat")] ChamDiem chamDiem)
        {
            int currentUserId = GetUserId();

            // Lấy thông tin giáo viên dựa trên UserID
            var teacher = _context.GiaoViens.FirstOrDefault(s => s.UserId == currentUserId);

            if (teacher != null)
            {
                chamDiem.MaGiaoVien = teacher.MaGiaoVien; // Gán MaGiaoVien với giáo viên đăng nhập
            }
            else
            {
                // Xử lý khi không tìm thấy thông tin giáo viên
                // Ví dụ: Redirect hoặc thông báo lỗi
                return RedirectToAction("Error");
            }

            // Kiểm tra xem sinh viên đã được chấm điểm chưa
            var existingGrade = _context.ChamDiems
                .FirstOrDefault(g => g.MaSinhVien == chamDiem.MaSinhVien && g.MaKyNang == chamDiem.MaKyNang && g.MaGiaoVien == chamDiem.MaGiaoVien);

            if (existingGrade != null)
            {
                // Xử lý khi sinh viên đã được chấm điểm
                ModelState.AddModelError("MaSinhVien", "Sinh viên này đã được chấm điểm cho kỹ năng này.");
            }

            if (ModelState.IsValid)
            {
                // Tìm bản ghi có id lớn nhất trong bảng DangKyLopKyNangs
                var maxId = _context.ChamDiems.Max(x => x.Id);

                // Tạo mới một bản ghi với Id tăng dần
                chamDiem.Id = maxId + 1;
                _context.Add(chamDiem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Truyền lại SelectList cho MaKyNang và MaSinhVien
            ViewData["MaKyNang"] = new SelectList(_context.KyNangs, "MaKyNang", "MaKyNang", chamDiem.MaKyNang);
            ViewData["MaSinhVien"] = new SelectList(_context.SinhViens, "MaSinhVien", "MaSinhVien", chamDiem.MaSinhVien);
            return View(chamDiem);
        }

        // GET: Teacher/ChamDiems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ChamDiems == null)
            {
                return NotFound();
            }

            var chamDiem = await _context.ChamDiems.FindAsync(id);
            if (chamDiem == null)
            {
                return NotFound();
            }
            ViewData["MaGiaoVien"] = new SelectList(_context.GiaoViens, "MaGiaoVien", "MaGiaoVien", chamDiem.MaGiaoVien);
            ViewData["MaKyNang"] = new SelectList(_context.KyNangs, "MaKyNang", "MaKyNang", chamDiem.MaKyNang);
            ViewData["MaSinhVien"] = new SelectList(_context.SinhViens, "MaSinhVien", "MaSinhVien", chamDiem.MaSinhVien);
            return View(chamDiem);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaGiaoVien,MaSinhVien,MaKyNang,Dat")] ChamDiem chamDiem)
        {
            if (id != chamDiem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chamDiem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChamDiemExists(chamDiem.Id))
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
            ViewData["MaGiaoVien"] = new SelectList(_context.GiaoViens, "MaGiaoVien", "MaGiaoVien", chamDiem.MaGiaoVien);
            ViewData["MaKyNang"] = new SelectList(_context.KyNangs, "MaKyNang", "MaKyNang", chamDiem.MaKyNang);
            ViewData["MaSinhVien"] = new SelectList(_context.SinhViens, "MaSinhVien", "MaSinhVien", chamDiem.MaSinhVien);
            return View(chamDiem);
        }

        // GET: Teacher/ChamDiems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ChamDiems == null)
            {
                return NotFound();
            }

            var chamDiem = await _context.ChamDiems
                .Include(c => c.MaGiaoVienNavigation)
                .Include(c => c.MaKyNangNavigation)
                .Include(c => c.MaSinhVienNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chamDiem == null)
            {
                return NotFound();
            }

            return View(chamDiem);
        }

        // POST: Teacher/ChamDiems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ChamDiems == null)
            {
                return Problem("Entity set 'SchoolContext.ChamDiems'  is null.");
            }
            var chamDiem = await _context.ChamDiems.FindAsync(id);
            if (chamDiem != null)
            {
                _context.ChamDiems.Remove(chamDiem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChamDiemExists(int id)
        {
            return (_context.ChamDiems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
