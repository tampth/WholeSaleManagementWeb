using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WholeSaleManagementApp.Data;
using WholeSaleManagementApp.Models;

namespace WholeSaleManagementApp.Areas.admin.Controllers
{
    [Area("admin")]
    public class DealsController : Controller
    {
        private readonly MyDbContext _context;

        private readonly UserManager<AppUser> _userManager;

        public DealsController(MyDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: admin/Deals
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.Deals.Include(d => d.Account).Include(d => d.Contact);
            return View(await myDbContext.ToListAsync());
        }

        // GET: admin/Deals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deal = await _context.Deals
                .Include(d => d.Account)
                .Include(d => d.Contact)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deal == null)
            {
                return NotFound();
            }

            return View(deal);
        }

        // GET: admin/Deals/Create
        public IActionResult Create()
        {
            ViewData["ContactID"] = new SelectList(_context.Contacts, "Id", "FullName");
            return View();
        }

        // POST: admin/Deals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ContactID,EstimateCost")] Deal deal)
        {
            if (ModelState.IsValid)
            {

                var user = await _userManager.GetUserAsync(this.User);
                deal.OwnerID = user.Id;
                deal.Stage = Stage.A;
                deal.ActualCost = 0;
                deal.AccountID = (int)_context.Contacts.Find(deal.ContactID).AccountID;


                _context.Add(deal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ContactID"] = new SelectList(_context.Contacts, "Id", "Email", deal.ContactID);
            return View(deal);
        }

        // GET: admin/Deals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deal = await _context.Deals.FindAsync(id);
            if (deal == null)
            {
                return NotFound();
            }
            ViewData["AccountID"] = new SelectList(_context.Accounts, "Id", "Name", deal.AccountID);
            ViewData["ContactID"] = new SelectList(_context.Contacts, "Id", "Email", deal.ContactID);
            return View(deal);
        }

        // POST: admin/Deals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AccountID,ContactID,OwnerID,Stage,EstimateCost,ActualCost")] Deal deal)
        {
            if (id != deal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(deal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DealExists(deal.Id))
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
            ViewData["AccountID"] = new SelectList(_context.Accounts, "Id", "Name", deal.AccountID);
            ViewData["ContactID"] = new SelectList(_context.Contacts, "Id", "Email", deal.ContactID);
            return View(deal);
        }

        // GET: admin/Deals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deal = await _context.Deals
                .Include(d => d.Account)
                .Include(d => d.Contact)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deal == null)
            {
                return NotFound();
            }

            return View(deal);
        }

        // POST: admin/Deals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deal = await _context.Deals.FindAsync(id);
            _context.Deals.Remove(deal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DealExists(int id)
        {
            return _context.Deals.Any(e => e.Id == id);
        }
    }
}
