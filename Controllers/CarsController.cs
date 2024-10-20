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
    public class CarsController : Controller
    {
        private readonly TransportCompanyContext _context;

        public CarsController(TransportCompanyContext context)
        {
            _context = context;
        }

        // GET: Cars1
        public async Task<IActionResult> Index()
        {
            var transportCompanyContext = _context.Cars.Include(c => c.Brand).Include(c => c.CarType).Include(c => c.Driver).Include(c => c.Mechanic);
            return View(await transportCompanyContext.ToListAsync());
        }

        // GET: Cars1/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Cars1/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.CarBrands, "Id", "Id");
            ViewData["CarTypeId"] = new SelectList(_context.CarTypes, "Id", "Id");
            ViewData["DriverId"] = new SelectList(_context.Employees, "Id", "Id");
            ViewData["MechanicId"] = new SelectList(_context.Employees, "Id", "Id");
            return View();
        }

        // POST: Cars1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Cars1/Edit/5
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
            ViewData["BrandId"] = new SelectList(_context.CarBrands, "Id", "Id", car.BrandId);
            ViewData["CarTypeId"] = new SelectList(_context.CarTypes, "Id", "Id", car.CarTypeId);
            ViewData["DriverId"] = new SelectList(_context.Employees, "Id", "Id", car.DriverId);
            ViewData["MechanicId"] = new SelectList(_context.Employees, "Id", "Id", car.MechanicId);
            return View(car);
        }

        // POST: Cars1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Cars1/Delete/5
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

        // POST: Cars1/Delete/5
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
