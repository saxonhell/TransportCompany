using System;
using System.Collections.Generic;
using System.Drawing.Printing;
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
    public class CargoTypesController : Controller
    {
        private readonly TransportCompanyContext _context;
        private readonly CachedDataService _cachedDataService;

        public CargoTypesController(TransportCompanyContext context, CachedDataService cachedDataService)
        {
            _context = context;
            _cachedDataService = cachedDataService;
        }

        // GET: CargoTypes
        public async Task<IActionResult> Index(string nameFilter, int? carTypeFilter, int page = 1, int pageSize = 20)
        {
            var modelsQuery = _cachedDataService.GetCargoTypes(); // Получаем все записи

            // Фильтрация
            if (!string.IsNullOrEmpty(nameFilter))
            {
                modelsQuery = modelsQuery.Where(cargoType => cargoType.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (carTypeFilter.HasValue)
            {
                modelsQuery = modelsQuery.Where(cargoType => cargoType.CarTypeId == carTypeFilter.Value);
            }

            // Пагинация
            int totalItems = modelsQuery.Count(); // Общее количество записей
            var cargoTypes = modelsQuery
                .Skip((page - 1) * pageSize) // Пропускаем записи для предыдущих страниц
                .Take(pageSize) // Берем только записи текущей страницы
                .ToList();

            // Передаем данные во ViewBag для сохранения фильтров и создания пагинации
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.NameFilter = nameFilter;
            ViewBag.CarTypeFilter = carTypeFilter;

            // Передаем список типов автомобилей для фильтрации
            ViewBag.CarTypes = new SelectList(_context.CarTypes, "Id", "Name", carTypeFilter);

            return View(cargoTypes);
        }


        // GET: CargoTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargoType = _cachedDataService.GetCargoTypes()
                .FirstOrDefault(m => m.Id == id);
            if (cargoType == null)
            {
                return NotFound();
            }

            return View(cargoType);
        }

        // GET: CargoTypes/Create
        public IActionResult Create()
        {
            ViewData["CarTypeId"] = new SelectList(_context.CarTypes, "Id", "Name");
            return View();
        }

        // POST: CargoTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CarTypeId,Description")] CargoType cargoType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cargoType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarTypeId"] = new SelectList(_context.CarTypes, "Id", "Id", cargoType.CarTypeId);
            return View(cargoType);
        }

        // GET: CargoTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargoType = await _context.CargoTypes.FindAsync(id);
            if (cargoType == null)
            {
                return NotFound();
            }
            ViewData["CarTypeId"] = new SelectList(_context.CarTypes, "Id", "Name", cargoType.CarTypeId);
            return View(cargoType);
        }

        // POST: CargoTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CarTypeId,Description")] CargoType cargoType)
        {
            if (id != cargoType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cargoType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CargoTypeExists(cargoType.Id))
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
            ViewData["CarTypeId"] = new SelectList(_context.CarTypes, "Id", "Id", cargoType.CarTypeId);
            return View(cargoType);
        }

        // GET: CargoTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargoType = await _context.CargoTypes
                .Include(c => c.CarType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cargoType == null)
            {
                return NotFound();
            }

            return View(cargoType);
        }

        // POST: CargoTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cargoType = await _context.CargoTypes.FindAsync(id);
            if (cargoType != null)
            {
                _context.CargoTypes.Remove(cargoType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CargoTypeExists(int id)
        {
            return _context.CargoTypes.Any(e => e.Id == id);
        }
    }
}
