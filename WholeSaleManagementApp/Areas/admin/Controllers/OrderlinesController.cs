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
    public class OrderlinesController : Controller
    {
        private readonly MyDbContext _context;

        public OrderlinesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: admin/Orderlines
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.Orderlines.Include(o => o.Product);
            return View(await myDbContext.ToListAsync());
        }

        // GET: admin/Orderlines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderline = await _context.Orderlines
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderline == null)
            {
                return NotFound();
            }

            return View(orderline);
        }

        // GET: admin/Orderlines/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
            return View();
        }

        // POST: admin/Orderlines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,QuotationId,ProductId,UnitPrice,Quantity,Discount")] Orderline orderline)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderline);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", orderline.ProductId);
            return View(orderline);
        }

        // GET: admin/Orderlines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderline = await _context.Orderlines.FindAsync(id);
            if (orderline == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", orderline.ProductId);
            return View(orderline);
        }

        // POST: admin/Orderlines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,QuotationId,ProductId,UnitPrice,Quantity,Discount")] Orderline orderline)
        {
            if (id != orderline.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderline);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderlineExists(orderline.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", orderline.ProductId);
            return View(orderline);
        }

        // GET: admin/Orderlines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderline = await _context.Orderlines
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderline == null)
            {
                return NotFound();
            }

            return View(orderline);
        }

        // POST: admin/Orderlines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderline = await _context.Orderlines.FindAsync(id);
            _context.Orderlines.Remove(orderline);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderlineExists(int id)
        {
            return _context.Orderlines.Any(e => e.Id == id);
        }
    }
}
