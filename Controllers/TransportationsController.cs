using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TransportCompany.Data;
using System.Threading.Tasks;
using System;

namespace TransportCompany.Controllers
{
    public class TransportationsController : Controller
    {
        private readonly TransportCompanyContext _context;

        public TransportationsController(TransportCompanyContext context)
        {
            _context = context;
        }

        // GET: Transportations
        public async Task<IActionResult> Index(int? driverId, int? carId, string startDate, string endDate, int page = 1, int pageSize = 20)
        {
            var query = _context.Trips.Include(t => t.Car)
                                      .Include(t => t.Driver)
                                      .Include(t => t.Cargo)
                                      .AsQueryable();

            DateOnly? parsedStartDate = null;
            DateOnly? parsedEndDate = null;

            if (DateOnly.TryParse(startDate, out var tempStartDate))
            {
                parsedStartDate = tempStartDate;
            }

            if (DateOnly.TryParse(endDate, out var tempEndDate))
            {
                parsedEndDate = tempEndDate;
            }

            // Фильтрация по водителю и автомобилю
            if (driverId.HasValue)
            {
                query = query.Where(t => t.DriverId == driverId.Value);
            }

            if (carId.HasValue)
            {
                query = query.Where(t => t.CarId == carId.Value);
            }

            // Фильтрация по датам
            if (parsedStartDate.HasValue)
            {
                query = query.Where(t => t.DepartureDate >= parsedStartDate.Value);
            }

            if (parsedEndDate.HasValue)
            {
                query = query.Where(t => t.ArrivalDate <= parsedEndDate.Value);
            }

            // Пагинация
            int totalItems = await query.CountAsync();
            var transportations = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Передаем данные во ViewBag для фильтров и пагинации
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.DriverId = driverId;
            ViewBag.CarId = carId;
            ViewBag.StartDate = parsedStartDate;
            ViewBag.EndDate = parsedEndDate;

            // Список водителей и автомобилей для фильтрации
            ViewBag.Drivers = new SelectList(await _context.Employees.Where(e => e.Role == "Driver").ToListAsync(), "Id", "Name", driverId);
            ViewBag.Cars = new SelectList(await _context.Cars.ToListAsync(), "Id", "RegistrationNumber", carId);

            return View(transportations);
        }


    }
}
