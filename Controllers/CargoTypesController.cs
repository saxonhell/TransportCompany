using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TransportCompanyWeb.Data;
using TransportCompanyWeb.Models;

namespace TransportCompany.Controllers
{
    public class CargoTypesController : Controller
    {
        private readonly TransportCompanyContext _context;

        public CargoTypesController(TransportCompanyContext context)
        {
            _context = context;
        }

        // GET: CargoTypes
        public async Task<IActionResult> Index()
        {
            var transportCompanyContext = _context.CargoTypes.Include(c => c.CarType);
            return View(await transportCompanyContext.ToListAsync());
        }

        // GET: CargoTypes/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: CargoTypes/Create
        public IActionResult Create()
        {
            ViewData["CarTypeId"] = new SelectList(_context.CarTypes, "Id", "Id");
            return View();
        }

        // POST: CargoTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            ViewData["CarTypeId"] = new SelectList(_context.CarTypes, "Id", "Id", cargoType.CarTypeId);
            return View(cargoType);
        }

        // POST: CargoTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
