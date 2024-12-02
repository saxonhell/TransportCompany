using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TransportCompany.Data;

namespace TransportCompany.Controllers
{
    public class FleetController : Controller
    {
        private readonly TransportCompanyContext _context;

        public FleetController(TransportCompanyContext context)
        {
            _context = context;
        }

        // GET: Fleet
        public async Task<IActionResult> Index(int? carTypeFilter, int? carBrandFilter, int page = 1, int pageSize = 20)
        {
            var query = _context.Cars.AsQueryable();

            // Фильтрация по типу автомобиля
            if (carTypeFilter.HasValue)
            {
                query = query.Where(car => car.CarTypeId == carTypeFilter.Value);
            }

            // Фильтрация по бренду автомобиля
            if (carBrandFilter.HasValue)
            {
                query = query.Where(car => car.BrandId == carBrandFilter.Value);
            }

            // Пагинация
            int totalItems = await query.CountAsync();
            var cars = await query
                .Skip((page - 1) * pageSize)
                .Include(c => c.CarType)
                .Include(c => c.Brand)
                .Include(c => c.Driver)
                .Include(c => c.Mechanic)
                .Take(pageSize)
                .ToListAsync();

            // Передаем данные во ViewBag для фильтров и пагинации
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.CarTypeFilter = carTypeFilter;
            ViewBag.CarBrandFilter = carBrandFilter;

            // Для фильтрации: Список типов и марок автомобилей
            ViewBag.CarTypes = new SelectList(await _context.CarTypes.ToListAsync(), "Id", "Name", carTypeFilter);
            ViewBag.CarBrands = new SelectList(await _context.CarBrands.ToListAsync(), "Id", "Name", carBrandFilter);

            return View(cars);
        }
    }
}
