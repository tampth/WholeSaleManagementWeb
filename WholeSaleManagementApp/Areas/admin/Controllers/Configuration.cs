using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WholeSaleManagementApp.Data;

namespace WholeSaleManagementApp.Areas.admin.Controllers
{
    [Area("admin")]
    public class Configuration : Controller
    {
        private readonly MyDbContext _dbContext;

        public Configuration(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        } 

        [HttpGet]
        public IActionResult DeleteDb()
        {
            return View();
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpPost]
        public async Task<IActionResult> DeleteDbAsync()
        {

            var success = await _dbContext.Database.EnsureDeletedAsync();

            StatusMessage = success ? "Xoa thanh cong" : "Khong xoa duoc";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Migrate()
        {
            await _dbContext.Database.MigrateAsync();

            StatusMessage = "Cap nhat thanh cong";

            return RedirectToAction(nameof(Index));
        }
    }
}
