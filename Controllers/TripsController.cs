using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TransportCompany.Data;
using TransportCompany.Models;
using TransportCompany.Service;

namespace TransportCompany.Controllers
{
    public class TripsController : Controller
    {
        private readonly TransportCompanyContext _context;

        public TripsController(TransportCompanyContext context)
        {
            _context = context;
        }

        // GET: Trips
        public async Task<IActionResult> Index(string customerFilter, string originFilter, string destinationFilter, bool? paymentStatusFilter, bool? returnStatusFilter, int page = 1, int pageSize = 20)
        {
            var modelsQuery = _context.Trips.AsQueryable();
               
            // Фильтрация
            if (!string.IsNullOrEmpty(customerFilter))
            {
                modelsQuery = modelsQuery.Where(trip =>
                    EF.Functions.Like(trip.Customer, $"%{customerFilter}%"));
            }

            if (!string.IsNullOrEmpty(originFilter))
            {
                modelsQuery = modelsQuery.Where(trip =>
                    EF.Functions.Like(trip.Origin, $"%{originFilter}%"));
            }

            if (!string.IsNullOrEmpty(destinationFilter))
            {
                modelsQuery = modelsQuery.Where(trip =>
                    EF.Functions.Like(trip.Destination, $"%{destinationFilter}%"));
            }

            if (paymentStatusFilter.HasValue)
            {
                modelsQuery = modelsQuery.Where(trip => trip.PaymentStatus == paymentStatusFilter.Value);
            }

            if (returnStatusFilter.HasValue)
            {
                modelsQuery = modelsQuery.Where(trip => trip.ReturnStatus == returnStatusFilter.Value);
            }

            // Пагинация
            int totalItems = await modelsQuery.CountAsync(); // Общее количество записей
            var trips = await modelsQuery
                .Skip((page - 1) * pageSize)
                .Include(t => t.Car)
                .ThenInclude(c => c.Brand)
                .Include(t => t.Cargo)
                .Include(t => t.Driver)// Пропускаем записи для предыдущих страниц
                .Take(pageSize) // Берем только записи текущей страницы
                .ToListAsync();

            // Передаем данные во ViewBag для сохранения фильтров и создания пагинации
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.CustomerFilter = customerFilter;
            ViewBag.OriginFilter = originFilter;
            ViewBag.DestinationFilter = destinationFilter;
            ViewBag.PaymentStatusFilter = paymentStatusFilter;
            ViewBag.ReturnStatusFilter = returnStatusFilter;

            return View(trips);
        }

        // GET: Trips/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .Include(t => t.Car)
                .Include(t => t.Cargo)
                .Include (t => t.Driver)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }

        // GET: Trips/Create
        public IActionResult Create()
        {
            // Создаём SelectList для CarId с отображением бренда автомобиля
            ViewData["CarId"] = new SelectList(
                _context.Cars.Include(c => c.Brand)
                .ToList(),
                "Id", "Brand.Name");

            // Создаём SelectList для CargoId с отображением имени груза
            ViewData["CargoId"] = new SelectList(
                _context.Cargos.ToList(),
                "Id", "Name");

            // Создаём SelectList для DriverId с отображением имени водителя
            ViewData["DriverId"] = new SelectList(
                _context.Employees.ToList(),
                "Id", "Name");

            // Для отображения BooleanOptions
            ViewData["BooleanOptions"] = new SelectList(new[]
            {
                new { Value = true, Text = "True" },
                new { Value = false, Text = "False" }
            }, "Value", "Text");

            return View();
        }


        // POST: Trips/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CarId,Customer,Origin,Destination,DepartureDate,ArrivalDate,CargoId,Price,PaymentStatus,ReturnStatus,DriverId")] Trip trip)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trip);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Id", trip.CarId);
            ViewData["CargoId"] = new SelectList(_context.Cargos, "Id", "Id", trip.CargoId);
            ViewData["DriverId"] = new SelectList(_context.Employees, "Id", "Id", trip.DriverId);

            return View(trip);
        }

        // GET: Trips/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .Include(t => t.Car)
                .Include(t => t.Cargo)
                .Include(t => t.Driver)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (trip == null)
            {
                return NotFound();
            }

            ViewData["CarId"] = new SelectList(await _context.Cars
                .Include(c => c.Brand)
                .ToListAsync(),
                "Id", "Brand.Name", trip.CarId);

            ViewData["CargoId"] = new SelectList(await _context.Cargos.ToListAsync(), "Id", "Name", trip.CargoId);
            ViewData["DriverId"] = new SelectList(await _context.Employees.ToListAsync(), "Id", "Name", trip.DriverId);

            // Для отображения BooleanOptions
            ViewData["BooleanOptions"] = new SelectList(new[]
            {
                new { Value = true, Text = "True" },
                new { Value = false, Text = "False" }
            }, "Value", "Text");

            return View(trip);
        }




        // POST: Trips/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CarId,Customer,Origin,Destination,DepartureDate,ArrivalDate,CargoId,Price,PaymentStatus,ReturnStatus,DriverId")] Trip trip)
        {
            if (id != trip.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TripExists(trip.Id))
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

            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Id", trip.CarId);
            ViewData["CargoId"] = new SelectList(_context.Cargos, "Id", "Id", trip.CargoId);
            ViewData["DriverId"] = new SelectList(_context.Employees, "Id", "Id", trip.DriverId);

            return View(trip);
        }

        // GET: Trips/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .Include(t => t.Car)
                .Include(t => t.Cargo)
                .Include(t => t.Driver)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }

        // POST: Trips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trip = await _context.Trips
                .Include(t => t.Car)
                .Include(t => t.Cargo)
                .Include(t => t.Driver)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trip != null)
            {
                _context.Trips.Remove(trip);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool TripExists(int id)
        {
            return _context.Trips.Any(e => e.Id == id);
        }
    }
}
