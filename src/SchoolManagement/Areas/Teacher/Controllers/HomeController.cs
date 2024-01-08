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

    public class HomeController : Controller
    {
        private SchoolContext _ctx;
        private UserDAO _userDAO;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HomeController(
            SchoolContext ctx, UserDAO userDAO, IHttpContextAccessor httpContextAccessor
            )
        {

            _ctx = ctx;
            _userDAO = userDAO;
            _httpContextAccessor = httpContextAccessor;


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
        public int GetUserId()
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

       
        public IActionResult Index()
        {
            return View();
        }
     


        public IActionResult ViewProfile()
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                // Người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login");
            }
            int userId = GetUserId(); // Lấy ID của giáo viên đăng nhập

            // Tìm giáo viên dựa trên ID của người dùng
            var teacher = _ctx.GiaoViens.FirstOrDefault(s => s.UserId == userId);

            if (teacher != null)
            {
                return View(teacher); // Trả về view để hiển thị thông tin cá nhân của giáo viên
            }

            // Xử lý trường hợp không tìm thấy giáo viên
            return RedirectToAction("Error", "Home");
        }


    }

}

