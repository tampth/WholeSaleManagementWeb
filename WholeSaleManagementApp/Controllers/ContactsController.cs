using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WholeSaleManagementApp.Data;
using WholeSaleManagementApp.Models.Contact;

namespace WholeSaleManagementApp.Controllers
{
    public class ContactsController : Controller
    {
        private readonly MyDbContext _context;

        public ContactsController(MyDbContext context)
        {
            _context = context;
        }

        [TempData]
        public string StatusMessage { get; set; }

        // GET: Contacts/Create
        [HttpGet]
        [AllowAnonymous]
        public IActionResult SendContact()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendContact([Bind("FullName,Email,Phone")] Contact contact)
        {
            if (ModelState.IsValid)
            {

                contact.DateSent = DateTime.Now;

                _context.Add(contact);
                await _context.SaveChangesAsync();

                StatusMessage = "Liên hệ của bạn đã được gửi thành công";

                return RedirectToAction("Index", "Home");
            }
            return View(contact);
        }
    }
}
