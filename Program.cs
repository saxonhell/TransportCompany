using Microsoft.EntityFrameworkCore;
using TransportCompanyWeb.Data;
using TransportCompanyWeb.Service;

namespace TransportCompany
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DBConnection");

            builder.Services.AddDbContext<TransportCompanyContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<CachedDataService>();

            // Добавление поддержки контроллеров с представлениями (MVC)
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Настройка маршрутизации
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.Run();
        }
    }
}

