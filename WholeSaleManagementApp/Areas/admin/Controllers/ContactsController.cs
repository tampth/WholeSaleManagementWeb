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

namespace WholeSaleManagementApp.Areas.admin.Controllers
{
    [Area("admin")]
    public class ContactsController : Controller
    {
        private readonly MyDbContext _context;

        public ContactsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: admin/Contacts
        public async Task<IActionResult> Index(string sortOrder, int page = 1, int ComID = 0)
        {
            var pageNumber = page;
            var pageSize = 5;

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            //ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            var ps = from p in _context.Contacts
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

            List<Contact> lsProducts = new List<Contact>();

            if (ComID != 0)
            {
                lsProducts = _context.Contacts.AsNoTracking()
                                              .Where(x => x.CompanyId == ComID)
                                              .Include(x => x.Company)
                                              .OrderByDescending(x => x.Id).ToList();
            }
            else
            {
                lsProducts = _context.Contacts.AsNoTracking()
                                              .Include(x => x.Company)
                                              .OrderByDescending(x => x.Id).ToList();
            }

            PagedList<Contact> models = new PagedList<Contact>(lsProducts.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentCateID = ComID;
            ViewBag.CurrentPage = pageNumber;


            ViewData["Company"] = new SelectList(_context.Companies, "Id", "Name", ComID);
            return View(models);
        }
        public IActionResult Filter(int ComID = 0)
        {
            var url = $"/admin/Contacts?ComID={ComID}";
            if (ComID == 0)
            {
                url = $"/admin/Contacts";
            }

            return Json(new { status = "success", redirectUrl = url });
        }

        // GET: admin/Contacts/Details/5
        public async Task<IActionResult> Details(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var contact = await _context.Contacts
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (contact == null)
                {
                    return NotFound();
                }

                return View(contact);
            }

            // GET: admin/Contacts/Create
            public IActionResult Create()
            {
                return View();
            }

            // POST: admin/Contacts/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create([Bind("Id,Email,Company,FullName,Phone")] Contact contact)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(contact);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(contact);
            }

            // GET: admin/Contacts/Edit/5
            public async Task<IActionResult> Edit(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var contact = await _context.Contacts.FindAsync(id);
                if (contact == null)
                {
                    return NotFound();
                }
                return View(contact);
            }

            // POST: admin/Contacts/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Company,FullName,Phone")] Contact contact)
            {
                if (id != contact.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(contact);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ContactExists(contact.Id))
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
                return View(contact);
            }

            // GET: admin/Contacts/Delete/5
            public async Task<IActionResult> Delete(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var contact = await _context.Contacts
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (contact == null)
                {
                    return NotFound();
                }

                return View(contact);
            }

            // POST: admin/Contacts/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var contact = await _context.Contacts.FindAsync(id);
                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            private bool ContactExists(int id)
            {
                return _context.Contacts.Any(e => e.Id == id);
            }
        }
    }
