using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WholeSaleManagementApp.Data;
using WholeSaleManagementApp.Models.Blog;

namespace WholeSaleManagementApp.Controllers
{
    public class BlogsController : Controller
    {
        private readonly MyDbContext _context;

        public BlogsController(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int CatID = 0)
        {
            var pageNumber = page;
            var pageSize = 15;

            var ps = from p in _context.Products
                     select p;

            List<PostCategory> lsProducts = new List<PostCategory>();

            if (CatID != 0)
            {
                lsProducts = _context.PostCategories.AsNoTracking()
                                              .Where(x => x.CategoryID == CatID)
                                              .Include(x => x.Category)
                                              .Include(x => x.Post)
                                              .OrderByDescending(x => x.PostID).ToList();
            }
            else
            {
                lsProducts = _context.PostCategories.AsNoTracking()
                                              .Include(x => x.Category)
                                              .Include(x => x.Post)
                                              .OrderByDescending(x => x.PostID).ToList();
            }

            PagedList<PostCategory> models = new PagedList<PostCategory>(lsProducts.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentCateID = CatID;
            ViewBag.CurrentPage = pageNumber;


            ViewData["DanhMuc"] = new SelectList(_context.CategoryBlogs, "Id", "Title", CatID);
            return View(models);
        }
        public IActionResult Filter(int CatID = 0)
        {
            var url = $"/Blogs?CatID={CatID}";
            if (CatID == 0)
            {
                url = $"/Blogs";
            }

            return Json(new { status = "success", redirectUrl = url });
        }

        public IActionResult Details(int id)
        {
            var product = _context.PostCategories.Include(x => x.Category).Include(x => x.Post).FirstOrDefault(x => x.PostID == id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }
            return View(product);
        }
    }
}
