using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{

    public class HomeController : Controller
    {
        private SchoolContext _ctx;
        private UserDAO _userDAO;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(SchoolContext ctx, UserDAO userDAO, IHttpContextAccessor httpContextAccessor)
        {
            _ctx = ctx;
            _userDAO = userDAO;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {

            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //Dang nhap
        [HttpPost]
        public IActionResult Login(User user)
        {
            User existingUser = _userDAO.GetUserByUserName(user.Username);

            if (existingUser == null || existingUser.Password != user.Password)
            {
                ModelState.AddModelError("LoginError", "Invalid username or password");
                return View();
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, existingUser.UserId.ToString())
    };

            if (existingUser.Role == "Admin")
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme))).Wait();
                return RedirectToAction("Index", "Admin");
            }
            if (existingUser.Role == "Teacher")
            {
                claims.Add(new Claim(ClaimTypes.Role, "Teacher"));
                HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme))).Wait();
                return RedirectToAction("Index", "Teacher");
            }
            HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme))).Wait();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Logout()
        {

            HttpContext.SignOutAsync(); // Đăng xuất người dùng

            // Chuyển hướng đến trang chủ 
            return RedirectToAction("Index");
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

        [HttpGet]
        public IActionResult RegisterSkill()
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                // Người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login");
            }
            // Fetch all available skills from the database
            var availableSkills = _ctx.KyNangs.ToList(); // Assuming KyNangs represents skills

            // Pass the available skills to the view for display in a dropdown, for instance
            ViewBag.Skills = new SelectList(availableSkills, "MaKyNang", "TenKyNang");

            return View();
        }

        [HttpPost]
        public IActionResult RegisterSkill(DangKyKyNang registration)
        {
            
            // Get the current user's ID
            int userId = GetUserId();

            // Fetch the student based on the logged-in user's ID
            var student = _ctx.SinhViens.FirstOrDefault(s => s.UserId == userId);

            if (student != null)
            {
                var maxId = _ctx.DangKyKyNangs.Max(x => x.Id);

                
                registration.Id = maxId + 1;
               
                registration.MaSinhVien = student.MaSinhVien;

                // Set ngày đăng ký là ngày hiện tại
                registration.NgayDangKy = DateTime.Now; // Đây là cách lấy ngày giờ hiện tại
                _ctx.DangKyKyNangs.Add(registration);
                _ctx.SaveChanges();

                return RedirectToAction("Index", "Home"); // Redirect to home or another appropriate action
            }

            // If student not found or other error occurs, handle accordingly
            return RedirectToAction("Error", "Home");
        }

        public IActionResult ViewResults()
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                // Người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login");
            }
            int userId = GetUserId(); // Lấy ID của sinh viên đăng nhập

            // Tìm sinh viên dựa trên ID của người dùng
            var student = _ctx.SinhViens.FirstOrDefault(s => s.UserId == userId);

            if (student != null)
            {
                // Lấy tất cả các kết quả của sinh viên đó từ bảng ChamDiem và include thông tin kỹ năng
                var studentResults = _ctx.ChamDiems
                    .Where(cd => cd.MaSinhVien == student.MaSinhVien)
                    .Include(cd => cd.MaKyNangNavigation) // Include thông tin kỹ năng
                    .ToList();

                return View(studentResults); // Trả về view để hiển thị kết quả của sinh viên
            }

            // Xử lý trường hợp không tìm thấy sinh viên
            return RedirectToAction("Error", "Home");
        }


        public IActionResult ViewProfile()
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                // Người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login");
            }
            int userId = GetUserId(); // Lấy ID của sinh viên đăng nhập

            // Tìm sinh viên dựa trên ID của người dùng
            var student = _ctx.SinhViens.FirstOrDefault(s => s.UserId == userId);

            if (student != null)
            {
                return View(student); // Trả về view để hiển thị thông tin cá nhân của sinh viên
            }

            // Xử lý trường hợp không tìm thấy sinh viên
            return RedirectToAction("Error", "Home");
        }

        public IActionResult ViewRegisteredSkills()
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                // Người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login");
            }
            int userId = GetUserId(); // Lấy ID của sinh viên đăng nhập

            // Tìm sinh viên dựa trên ID của người dùng
            var student = _ctx.SinhViens.FirstOrDefault(s => s.UserId == userId);

            if (student != null)
            {
                // Lấy danh sách kỹ năng đã đăng ký của sinh viên từ bảng DangKyKyNang
                var registeredSkills = _ctx.DangKyKyNangs
                    .Where(dkkn => dkkn.MaSinhVien == student.MaSinhVien)
                    .Select(dkkn => dkkn.MaKyNangNavigation)
                    .ToList();

                return View(registeredSkills); // Trả về view để hiển thị danh sách kỹ năng đã đăng ký của sinh viên
            }

            // Xử lý trường hợp không tìm thấy sinh viên
            return RedirectToAction("Error", "Home");
        }
        // Action để xóa kỹ năng đã đăng ký của sinh viên
        [HttpPost]
        public IActionResult RemoveRegisteredSkill(string skillId)
        {
            int userId = GetUserId(); // Lấy ID của sinh viên đăng nhập

            // Tìm sinh viên dựa trên ID của người dùng
            var student = _ctx.SinhViens.FirstOrDefault(s => s.UserId == userId);

            if (student != null)
            {
                // Xóa kỹ năng đã đăng ký của sinh viên dựa trên skillId
                var registeredSkill = _ctx.DangKyKyNangs.FirstOrDefault(dkkn => dkkn.MaSinhVien == student.MaSinhVien && dkkn.MaKyNang == skillId);

                if (registeredSkill != null)
                {
                    _ctx.DangKyKyNangs.Remove(registeredSkill);
                    _ctx.SaveChanges();
                }

                return RedirectToAction("ViewRegisteredSkills"); // Chuyển hướng về trang hiển thị danh sách kỹ năng đã đăng ký
            }

            // Xử lý trường hợp không tìm thấy sinh viên
            return RedirectToAction("Error", "Home");
        }


       



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}