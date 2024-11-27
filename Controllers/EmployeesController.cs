using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TransportCompany.Data;
using TransportCompany.Models;
using TransportCompany.Service;

namespace TransportCompany.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly CachedDataService _cachedDataService;
        private readonly TransportCompanyContext _context;

        public EmployeesController(CachedDataService cachedDataService, TransportCompanyContext transportCompanyContext)
        {
            _cachedDataService = cachedDataService;
            _context = transportCompanyContext;
        }

        // GET: Employees
        public async Task<IActionResult> Index(string nameFilter, string roleFilter, int page = 1, int pageSize = 20)
        {
            Console.WriteLine($"NameFilter: {nameFilter}, RoleFilter: {roleFilter}");
            var modelsQuery = _cachedDataService.GetEmployees(); // Получаем все записи

            // Фильтрация по имени
            if (!string.IsNullOrEmpty(nameFilter))
            {
                modelsQuery = modelsQuery.Where(employee => employee.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Фильтрация по роли
            if (!string.IsNullOrEmpty(roleFilter))
            {
                modelsQuery = modelsQuery.Where(employee => employee.Role == roleFilter);
            }

            // Пагинация
            int totalItems = modelsQuery.Count(); // Общее количество записей
            var employees = modelsQuery
                .Skip((page - 1) * pageSize) // Пропускаем записи для предыдущих страниц
                .Take(pageSize) // Берем только записи текущей страницы
                .ToList();

            // Передаем данные во ViewBag для сохранения фильтров
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.NameFilter = nameFilter;
            ViewBag.RoleFilter = roleFilter;

            // Передаем список ролей в выпадающий список
            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Driver", Text = "Driver", Selected = roleFilter == "Driver" },
                new SelectListItem { Value = "Mechanic", Text = "Mechanic", Selected = roleFilter == "Mechanic" }
            };

            return View(employees);
        }



        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = _cachedDataService.GetEmployees()
                .FirstOrDefault(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(new List<string> { "Driver", "Mechanic" });
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Role")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            // Передаем список ролей в ViewBag
            ViewBag.Roles = new SelectList(new List<string> { "Driver", "Mechanic" });

            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Role")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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

            // Повторно передаем список ролей в случае ошибки валидации
            ViewBag.Roles = new SelectList(new List<string> { "Driver", "Mechanic" });

            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
