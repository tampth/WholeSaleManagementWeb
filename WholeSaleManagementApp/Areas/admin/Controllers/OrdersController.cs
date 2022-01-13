using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using WholeSaleManagementApp.Data;
using WholeSaleManagementApp.Models;
using WholeSalerWeb.Models;

namespace WholeSaleManagementApp.Areas.admin.Controllers
{
    [Area("admin")]
    public class OrdersController : Controller
    {
        private readonly MyDbContext _context;

        public OrdersController(MyDbContext context)
        {
            _context = context;
        }

        // GET: admin/Orders
        public async Task<IActionResult> Index(string sortOrder, int page = 1, int CatID = 0)
        {
            var pageNumber = page;
            var pageSize = 20;

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            //ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            var ps = from p in _context.Orders
                     select p;

            switch (sortOrder)
            {
                case "name_desc":
                    ps = ps.OrderByDescending(s => s.FullName);
                    break;
                default:
                    ps = ps.OrderBy(s => s.FullName);
                    break;
            }

            List<Order> lsProducts = new List<Order>();

            if (CatID != 0)
            {
                lsProducts = _context.Orders.AsNoTracking()
                                              .Where(x => x.TransactStatusId == CatID)
                                              .Include(x => x.TransactStatus)
                                              .OrderByDescending(x => x.OrderId).ToList();
            }
            else
            {
                lsProducts = _context.Orders.AsNoTracking()
                                              .Include(x => x.TransactStatus)
                                              .OrderByDescending(x => x.OrderId).ToList();
            }

            PagedList<Order> models = new PagedList<Order>(lsProducts.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentCateID = CatID;
            ViewBag.CurrentPage = pageNumber;


            return View(models);
        }

        // GET: admin/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.TransactStatus)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: admin/Orders/Create
        public IActionResult Create()
        {
            ViewData["TransactStatusId"] = new SelectList(_context.Set<Transactstatus>(), "TransactStatusId", "TransactStatusId");
            return View();
        }

        // POST: admin/Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,UserId,TransactStatusId,OrderDate,ShipDate,Deleted,Paid,PaymentDate,PaymentId,Note,FullName,Address,Email,Phone,Total")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TransactStatusId"] = new SelectList(_context.Set<Transactstatus>(), "TransactStatusId", "TransactStatusId", order.TransactStatusId);
            return View(order);
        }

        // GET: admin/Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["TransactStatusId"] = new SelectList(_context.Set<Transactstatus>(), "TransactStatusId", "TransactStatusId", order.TransactStatusId);
            return View(order);
        }

        // POST: admin/Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,UserId,TransactStatusId,OrderDate,ShipDate,Deleted,Paid,PaymentDate,PaymentId,Note,FullName,Address,Email,Phone,Total")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["TransactStatusId"] = new SelectList(_context.Set<Transactstatus>(), "TransactStatusId", "TransactStatusId", order.TransactStatusId);
            return View(order);
        }

        // GET: admin/Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.TransactStatus)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
