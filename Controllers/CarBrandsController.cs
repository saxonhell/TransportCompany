﻿using System;
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
    public class CarBrandsController : Controller
    {
        private readonly TransportCompanyContext _context;

        public CarBrandsController(TransportCompanyContext context)
        {
            _context = context;
        }

        // GET: CarBrands
        public async Task<IActionResult> Index()
        {
            return View(await _context.CarBrands.ToListAsync());
        }

        // GET: CarBrands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carBrand = await _context.CarBrands
                .FirstOrDefaultAsync(m => m.Id == id);
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            var carBrand = await _context.CarBrands.FindAsync(id);
            if (carBrand != null)
            {
                _context.CarBrands.Remove(carBrand);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarBrandExists(int id)
        {
            return _context.CarBrands.Any(e => e.Id == id);
        }
    }
}
