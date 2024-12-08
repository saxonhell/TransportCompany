using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TransportCompany.Data;
using TransportCompany.Middleware;
using TransportCompany.Models;
using TransportCompany.Service;
using TransportCompany.Data.Initializer;
using Microsoft.AspNetCore.Authorization;
using TransportCompany.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DBConnection")
    ?? throw new InvalidOperationException("Строка подключения не найдена.");


// Регистрация TransportCompanyContext
builder.Services.AddDbContext<TransportCompanyContext>(options =>
    options.UseSqlServer(connectionString));

// Регистрация ApplicationDbContext для Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddMemoryCache();
builder.Services.AddScoped<CachedDataService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login"; // Путь для перенаправления на страницу входа
    options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // Путь для страницы с ограничением доступа
});


builder.Services.AddControllersWithViews(options =>
{
    // Добавляем глобальную политику авторизации
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Identity/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Identity/Account/Register");
    options.Conventions.AllowAnonymousToPage("/Identity/Account/AccessDenied");
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax; // Обновите настройки cookies
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

// Настройка middleware
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession(); // Сессии пользователя


// Настройка маршрутов
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
