using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using SchoolManagement.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = "YourCookieName";
    options.Cookie.HttpOnly = true;
    options.LoginPath = "/home/login";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Thời gian hết hạn của cookie

});
// Add services to the container.

builder.Services.AddAuthorization();
builder.Services.AddSession();
builder.Services.AddScoped<UserDAO>();
//Dependency Injection
builder.Services.AddDbContext<SchoolContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("School"));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapAreaControllerRoute(
    name: "MyAreas",
    areaName: "Teacher",
    pattern: "Teacher/{action=Index}/{id?}",
    defaults: new { controller = "Home", action = "Index" });
app.MapAreaControllerRoute(
    name: "MyAreas",
    areaName: "Teacher",
    pattern: "Teacher/DangKyLopKyNangs/{action=Index}/{id?}",
    defaults: new { controller = "DangKyLopKyNangs", action = "Index" });
app.MapAreaControllerRoute(
    name: "MyAreas",
    areaName: "Teacher",
    pattern: "Teacher/ChamDiems/{action=Index}/{id?}",
    defaults: new { controller = "ChamDiems", action = "Index" });
app.MapAreaControllerRoute(
    name: "MyAreas2",
    areaName: "Admin",
    pattern: "Admin/{action=Index}/{id?}",
    defaults: new { controller = "Home", action = "Index" });
app.MapAreaControllerRoute(
    name: "MyAreas2",
    areaName: "Admin",
    pattern: "Admin/GiaoViens/{action=Index}/{id?}",
    defaults: new { controller = "GiaoViens", action = "Index" });
app.MapAreaControllerRoute(
    name: "MyAreas2",
    areaName: "Admin",
    pattern: "Admin/SinhViens/{action=Index}/{id?}",
    defaults: new { controller = "SinhViens", action = "Index" });
app.MapAreaControllerRoute(
    name: "MyAreas2",
    areaName: "Admin",
    pattern: "Admin/Users/{action=Index}/{id?}",
    defaults: new { controller = "Users", action = "Index" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "removeSkill",
        pattern: "Home/RemoveRegisteredSkill/{skillId}", // Đảm bảo route phù hợp với đường dẫn bạn đang sử dụng
        defaults: new { controller = "Home", action = "RemoveRegisteredSkill" });
app.Run();
