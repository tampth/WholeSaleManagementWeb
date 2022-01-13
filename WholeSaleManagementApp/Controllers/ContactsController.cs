using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WholeSaleManagementApp.Data;
using WholeSaleManagementApp.Models;

namespace WholeSaleManagementApp.Controllers
{
    public class ContactsController : Controller
    {
        private readonly MyDbContext _context;

        public ContactsController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        // GET: Contacts/Create
        public IActionResult SendContact()
        {
            return View();
        }
        public IActionResult SentContact()
        {
            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendContact([Bind("Id,Email,CompanyName,FullName,Phone")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                foreach (Company item in _context.Companies)
                {
                    if (contact.CompanyName == item.Name)
                    {
                        contact.CompanyId = item.Id;
                        _context.Add(contact);
                    }    
                }
                if (contact.CompanyId == null)
                {
                    var company = new Company()
                    {
                        Name = contact.CompanyName
                    };
                    _context.Add(company);
                    await _context.SaveChangesAsync();
                    contact.CompanyId = _context.Companies.AsNoTracking().Where(x => x.Name == company.Name).FirstOrDefault().Id;
                    _context.Add(contact);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(SentContact));
            }
            return View(contact);
        }

    }
}
