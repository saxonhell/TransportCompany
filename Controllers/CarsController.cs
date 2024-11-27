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
    public class CarsController : Controller
    {
        private readonly TransportCompanyContext _context;
        private readonly CachedDataService _cachedDataService;

        public CarsController(TransportCompanyContext context, CachedDataService cachedDataService)
        {
            _context = context;
            _cachedDataService = cachedDataService;
        }

        // GET: Cars
        public async Task<IActionResult> Index(string sortField, string sortDirection, string brandFilter, int? yearFilter, 
                                               string driverFilter, int page = 1, int pageSize = 20)
        {
            var modelsQuery = _cachedDataService.GetCars(); // Получаем все записи

            // Фильтрация
            if (!string.IsNullOrEmpty(brandFilter))
            {
                modelsQuery = modelsQuery.Where(car => car.Brand.Name.Contains(brandFilter));
            }

            if (yearFilter.HasValue)
            {
                modelsQuery = modelsQuery.Where(car => car.YearOfManufacture == yearFilter.Value);
            }

            if (!string.IsNullOrEmpty(driverFilter))
            {
                modelsQuery = modelsQuery.Where(car => car.Driver.Name.Contains(driverFilter));
            }

            // Пагинация
            int totalItems = modelsQuery.Count();
            var cars = modelsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Передаем данные во ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.SortField = sortField;
            ViewBag.SortDirection = sortDirection;
            ViewBag.BrandFilter = brandFilter;
            ViewBag.YearFilter = yearFilter;
            ViewBag.DriverFilter = driverFilter;

            return View(cars);
        }



        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = _cachedDataService.GetCars()
                .FirstOrDefault(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Cars/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.CarBrands, "Id", "Name");
            ViewData["CarTypeId"] = new SelectList(_context.CarTypes, "Id", "Name");
            ViewData["DriverId"] = new SelectList(_context.Employees, "Id", "Name");
            ViewData["MechanicId"] = new SelectList(_context.Employees, "Id", "Name");
            return View();
        }

        // POST: Cars/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BrandId,CarTypeId,RegistrationNumber,BodyNumber,EngineNumber,YearOfManufacture,DriverId,LastMaintenanceDate,MechanicId")] Car car)
        {
            if (ModelState.IsValid)
            {
                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.CarBrands, "Id", "Id", car.BrandId);
            ViewData["CarTypeId"] = new SelectList(_context.CarTypes, "Id", "Id", car.CarTypeId);
            ViewData["DriverId"] = new SelectList(_context.Employees, "Id", "Id", car.DriverId);
            ViewData["MechanicId"] = new SelectList(_context.Employees, "Id", "Id", car.MechanicId);
            return View(car);
        }

        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.CarBrands, "Id", "Name", car.BrandId);
            ViewData["CarTypeId"] = new SelectList(_context.CarTypes, "Id", "Name", car.CarTypeId);
            ViewData["DriverId"] = new SelectList(_context.Employees, "Id", "Name", car.DriverId);
            ViewData["MechanicId"] = new SelectList(_context.Employees, "Id", "Name", car.MechanicId);
            return View(car);
        }

        // POST: Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BrandId,CarTypeId,RegistrationNumber,BodyNumber,EngineNumber,YearOfManufacture,DriverId,LastMaintenanceDate,MechanicId")] Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
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
            ViewData["BrandId"] = new SelectList(_context.CarBrands, "Id", "Id", car.BrandId);
            ViewData["CarTypeId"] = new SelectList(_context.CarTypes, "Id", "Id", car.CarTypeId);
            ViewData["DriverId"] = new SelectList(_context.Employees, "Id", "Id", car.DriverId);
            ViewData["MechanicId"] = new SelectList(_context.Employees, "Id", "Id", car.MechanicId);
            return View(car);
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.Brand)
                .Include(c => c.CarType)
                .Include(c => c.Driver)
                .Include(c => c.Mechanic)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}
