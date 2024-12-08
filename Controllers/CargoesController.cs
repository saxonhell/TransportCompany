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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TransportCompany.Controllers
{
    public class CargoesController : Controller
    {
        private readonly TransportCompanyContext _context;

        public CargoesController(TransportCompanyContext context)
        {
            _context = context;
        }

        // GET: Cargoes
        public async Task<IActionResult> Index(string nameFilter, int? cargoTypeFilter, int page = 1, int pageSize = 20)
        {
            var modelsQuery = _context.Cargos.AsQueryable(); // Получаем все записи

            // Фильтрация
            if (!string.IsNullOrEmpty(nameFilter))
            {
                modelsQuery = modelsQuery.Where(cargo =>
                    EF.Functions.Like(cargo.Name, $"%{nameFilter}%"));
            }

            if (cargoTypeFilter.HasValue)
            {
                modelsQuery = modelsQuery.Where(cargo => cargo.CargoTypeId == cargoTypeFilter.Value);
            }

            // Пагинация
            int totalItems = modelsQuery.Count(); // Общее количество записей
            var cargoes = modelsQuery
                .Skip((page - 1) * pageSize) // Пропускаем записи для предыдущих страниц
                .Include(c => c.CargoType)
                .Take(pageSize) // Берем только записи текущей страницы
                .ToList();

            // Передаем данные во ViewBag для сохранения фильтров и создания пагинации
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.NameFilter = nameFilter;
            ViewBag.CargoTypeFilter = cargoTypeFilter;

            // Передаем список типов грузов для фильтрации
            ViewBag.CargoTypes = new SelectList(_context.CargoTypes, "Id", "Name", cargoTypeFilter);

            return View(cargoes);
        }




        // GET: Cargoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargo = _context.Cargos
                .Include(c => c.CargoType)
                .FirstOrDefault(m => m.Id == id);
            if (cargo == null)
            {
                return NotFound();
            }

            return View(cargo);
        }

        // GET: Cargoes/Create
        public IActionResult Create()
        {
            ViewData["CargoTypeId"] = new SelectList(_context.CargoTypes, "Id", "Name");
            return View();
        }

        // POST: Cargoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CargoTypeId,ExpiryDate,Features")] Cargo cargo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cargo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CargoTypeId"] = new SelectList(_context.CargoTypes, "Id", "Name", cargo.CargoTypeId);
            return View(cargo);
        }

        // GET: Cargoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargo = await _context.Cargos.FindAsync(id);
            if (cargo == null)
            {
                return NotFound();
            }
            ViewData["CargoTypeId"] = new SelectList(_context.CargoTypes, "Id", "Name", cargo.CargoTypeId);
            return View(cargo);
        }


        // POST: Cargoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CargoTypeId,ExpiryDate,Features")] Cargo cargo)
        {
            if (id != cargo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cargo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CargoExists(cargo.Id))
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
            ViewData["CargoTypeId"] = new SelectList(_context.CargoTypes, "Id", "TypeName", cargo.CargoTypeId); // Повторяем передачу данных при ошибке валидации
            return View(cargo);
        }

        // GET: Cargoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cargo = await _context.Cargos
                .Include(c => c.Trips)  // Подключаем связанные поездки
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cargo == null)
            {
                return NotFound();
            }

            return View(cargo);
        }

        // POST: Cargoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cargo = await _context.Cargos
                .Include(c => c.Trips)  // Подключаем связанные поездки
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cargo != null)
            {
                // Удаляем все связанные поездки
                foreach (var trip in cargo.Trips.ToList())
                {
                    _context.Trips.Remove(trip);
                }

                // Удаляем сам груз
                _context.Cargos.Remove(cargo);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool CargoExists(int id)
        {
            return _context.Cargos.Any(e => e.Id == id);
        }
    }
}
