﻿using System;
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
    public class CarBrandsController : Controller
    {
        private readonly TransportCompanyContext _context;

        public CarBrandsController(TransportCompanyContext context)
        {
            _context = context;
        }

        // GET: CarBrands
        public async Task<IActionResult> Index(string nameFilter, string technicalSpecificationsFilter, int page = 1, int pageSize = 20)
        {
            var modelsQuery = _context.CarBrands.AsQueryable(); // Получаем записи

            // Фильтрация
            if (!string.IsNullOrEmpty(nameFilter))
            {
                modelsQuery = modelsQuery.Where(carBrand =>
                    EF.Functions.Like(carBrand.Name, $"%{nameFilter}%"));
            }

            if (!string.IsNullOrEmpty(technicalSpecificationsFilter))
            {
                modelsQuery = modelsQuery.Where(carBrand =>
                    EF.Functions.Like(carBrand.TechnicalSpecifications, $"%{technicalSpecificationsFilter}%"));
            }

            // Пагинация
            int totalItems = modelsQuery.Count(); // Общее количество записей
            var carBrands = modelsQuery
                .Skip((page - 1) * pageSize) // Пропускаем записи для предыдущих страниц
                .Take(pageSize) // Берем только записи текущей страницы
                .ToList();

            // Передаем данные во ViewBag для создания пагинации и сохранения фильтров
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.NameFilter = nameFilter;
            ViewBag.TechnicalSpecificationsFilter = technicalSpecificationsFilter;

            return View(carBrands);
        }


        // GET: CarBrands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carBrand = _context.CarBrands
                .FirstOrDefault(m => m.Id == id);
            if (carBrand == null)
            {
                return NotFound();
            }

            return View(carBrand);
        }

        // GET: CarBrands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CarBrands/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,TechnicalSpecifications,Description")] CarBrand carBrand)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carBrand);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(carBrand);
        }

        // GET: CarBrands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carBrand = await _context.CarBrands.FindAsync(id);
            if (carBrand == null)
            {
                return NotFound();
            }
            return View(carBrand);
        }

        // POST: CarBrands/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,TechnicalSpecifications,Description")] CarBrand carBrand)
        {
            if (id != carBrand.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carBrand);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarBrandExists(carBrand.Id))
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
            return View(carBrand);
        }

        // GET: CarBrands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carBrand = await _context.CarBrands
                .Include(cb => cb.Cars) // Подключаем связанные машины
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carBrand == null)
            {
                return NotFound();
            }

            return View(carBrand);
        }

        // POST: CarBrands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carBrand = await _context.CarBrands
                .Include(cb => cb.Cars)
                .ThenInclude(cb => cb.Trips)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (carBrand != null)
            {
                // Удаляем все автомобили, связанные с этим брендом
                foreach (var car in carBrand.Cars.ToList())
                {
                    // Удаляем все поездки, связанные с автомобилем
                    foreach (var trip in car.Trips.ToList())
                    {
                        _context.Trips.Remove(trip);
                    }

                    _context.Cars.Remove(car);  // Удаляем автомобиль
                }

                // Теперь можно удалить сам бренд
                _context.CarBrands.Remove(carBrand);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool CarBrandExists(int id)
        {
            return _context.CarBrands.Any(e => e.Id == id);
        }
    }
}
