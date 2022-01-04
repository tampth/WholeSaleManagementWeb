using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WholeSaleManagementApp.Data;
using WholeSaleManagementApp.Models;

namespace WholeSaleManagementApp.Areas.admin.Controllers
{
    [Area("admin")]
    public class QuotationsController : Controller
    {
        private readonly MyDbContext _context;

        public QuotationsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: admin/Quotations
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.Quotations.Include(q => q.Deal);
            return View(await myDbContext.ToListAsync());
        }

        // GET: admin/Quotations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quotation = await _context.Quotations
                .Include(q => q.Deal)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quotation == null)
            {
                return NotFound();
            }

            return View(quotation);
        }

        // GET: admin/Quotations/Create
        public IActionResult Create()
        {
            ViewData["DealId"] = new SelectList(_context.Deals, "Id", "Id");
            return View();
        }

        // POST: admin/Quotations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DealId,SalePersonId,Description,Amount")] Quotation quotation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quotation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DealId"] = new SelectList(_context.Deals, "Id", "Id", quotation.DealId);
            return View(quotation);
        }

        // GET: admin/Quotations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quotation = await _context.Quotations.FindAsync(id);
            if (quotation == null)
            {
                return NotFound();
            }
            ViewData["DealId"] = new SelectList(_context.Deals, "Id", "Id", quotation.DealId);
            return View(quotation);
        }

        // POST: admin/Quotations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DealId,SalePersonId,Description,Amount")] Quotation quotation)
        {
            if (id != quotation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quotation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuotationExists(quotation.Id))
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
            ViewData["DealId"] = new SelectList(_context.Deals, "Id", "Id", quotation.DealId);
            return View(quotation);
        }

        // GET: admin/Quotations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quotation = await _context.Quotations
                .Include(q => q.Deal)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quotation == null)
            {
                return NotFound();
            }

            return View(quotation);
        }

        // POST: admin/Quotations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quotation = await _context.Quotations.FindAsync(id);
            _context.Quotations.Remove(quotation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuotationExists(int id)
        {
            return _context.Quotations.Any(e => e.Id == id);
        }
    }
}
