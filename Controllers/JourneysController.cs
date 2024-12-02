using Microsoft.AspNetCore.Mvc;
using TransportCompany.Data;
using Microsoft.EntityFrameworkCore;

namespace TransportCompany.Controllers
{
    public class JourneysController : Controller
    {
        private readonly TransportCompanyContext _context;

        public JourneysController(TransportCompanyContext context)
        {
            _context = context;
        }

        // GET: Trips
        public async Task<IActionResult> Index(bool? isPaid, int page = 1, int pageSize = 20)
        {
            var query = _context.Trips.AsQueryable();

            // Фильтрация по статусу оплаты
            if (isPaid.HasValue)
            {
                query = query.Where(t => t.PaymentStatus == isPaid.Value);
            }

            // Пагинация
            int totalItems = await query.CountAsync();
            var trips = await query
                .Skip((page - 1) * pageSize)
                .Include(t => t.Car)
                .Include(t => t.Driver)
                .Take(pageSize)
                .ToListAsync();

            // Передаем данные во ViewBag для фильтров и пагинации
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.IsPaid = isPaid;

            return View(trips);
        }
    }
}
