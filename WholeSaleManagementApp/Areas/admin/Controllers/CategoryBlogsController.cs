using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WholeSaleManagementApp.Data;
using WholeSaleManagementApp.Models;

namespace WholeSaleManagementApp.Areas.admin.Controllers
{
    [Area("admin")]
    [Route("admin/blog/category/[action]/{id?}")]
    [Authorize(Roles = RoleName.Administrator)]
    public class CategoryBlogsController : Controller
    {
        private readonly MyDbContext _context;

        public CategoryBlogsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: admin/CategoryBlogs
        public async Task<IActionResult> Index()
        {
            var qr = (from c in _context.CategoryBlogs select c)
                    .Include(c => c.ParentCategory)
                    .Include(c => c.CategoryChildren);

            var categories = (await qr.ToListAsync())
                            .Where(c => c.ParentCategory == null)
                            .ToList();

            return View(categories);
        }

        // GET: admin/CategoryBlogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryBlog = await _context.CategoryBlogs
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoryBlog == null)
            {
                return NotFound();
            }

            return View(categoryBlog);
        }

        private void CreateSelectItems(List<CategoryBlog> source, List<CategoryBlog> des, int level)
        {
            string prefix = string.Concat(Enumerable.Repeat("--", level));
            foreach (var category in source)
            {
                //category.Title = prefix + category.Title;
                des.Add(new CategoryBlog()
                {
                    Id = category.Id,
                    Title = prefix + category.Title
                });
                if (category.CategoryChildren?.Count > 0)
                {
                    CreateSelectItems(category.CategoryChildren.ToList(), des, level + 1);
                }    
            }    
        }

        // GET: admin/CategoryBlogs/Create
        public async Task<IActionResult> CreateAsync()
        {
            var qr = (from c in _context.CategoryBlogs select c)
                    .Include(c => c.ParentCategory)
                    .Include(c => c.CategoryChildren);

            var categories = (await qr.ToListAsync())
                            .Where(c => c.ParentCategory == null)
                            .ToList();
            categories.Insert(0, new CategoryBlog()
            {
                Id = -1,
                Title = "Khong co danh muc cha"
            });

            var items = new List<CategoryBlog>();
            CreateSelectItems(categories, items, 0);

            var selectList = new SelectList(items, "Id", "Title");


            ViewData["ParentCategoryId"] = selectList;
            return View();
        }

        // POST: admin/CategoryBlogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ParentCategoryId,Title,Content,Slug")] CategoryBlog categoryBlog)
        {
            if (ModelState.IsValid)
            {
                if (categoryBlog.ParentCategoryId == -1) categoryBlog.ParentCategoryId = null;
                _context.Add(categoryBlog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Lay ra nhung danh muc null
            var qr = (from c in _context.CategoryBlogs select c)
                    .Include(c => c.ParentCategory)
                    .Include(c => c.CategoryChildren);

            var categories = (await qr.ToListAsync())
                            .Where(c => c.ParentCategory == null)
                            .ToList();
            categories.Insert(0, new CategoryBlog()
            {
                Id = -1,
                Title = "Khong co danh muc cha"
            });

            var items = new List<CategoryBlog>();
            CreateSelectItems(categories, items, 0);

            var selectList = new SelectList(items, "Id", "Title");

            ViewData["ParentCategoryId"] = selectList;
            return View(categoryBlog);
        }

        // GET: admin/CategoryBlogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryBlog = await _context.CategoryBlogs.FindAsync(id);
            if (categoryBlog == null)
            {
                return NotFound();
            }
            var qr = (from c in _context.CategoryBlogs select c)
                    .Include(c => c.ParentCategory)
                    .Include(c => c.CategoryChildren);

            var categories = (await qr.ToListAsync())
                            .Where(c => c.ParentCategory == null)
                            .ToList();
            categories.Insert(0, new CategoryBlog()
            {
                Id = -1,
                Title = "Khong co danh muc cha"
            });

            var items = new List<CategoryBlog>();
            CreateSelectItems(categories, items, 0);

            var selectList = new SelectList(items, "Id", "Title");

            ViewData["ParentCategoryId"] = selectList;
            return View(categoryBlog);
        }

        // POST: admin/CategoryBlogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ParentCategoryId,Title,Content,Slug")] CategoryBlog categoryBlog)
        {
            if (id != categoryBlog.Id)
            {
                return NotFound();
            }

            var canUpdate = true;

            if (categoryBlog.ParentCategoryId == categoryBlog.Id)
            {
                ModelState.AddModelError(string.Empty, "Phai chon danh muc khac");
                canUpdate = false;
            }    

            //Kiem tra thiet lap muc cha phu hop
            if (canUpdate && categoryBlog.ParentCategoryId != null)
            {
                var childCates = (from c in _context.CategoryBlogs select c)
                                .Include(c => c.CategoryChildren)
                                .ToList()
                                .Where(c => c.ParentCategoryId == categoryBlog.Id);
                Func<List<CategoryBlog>, bool> checkCateIds = null;
                checkCateIds = (cates) =>
                        {
                            foreach (var cate in cates)
                            {
                                Console.WriteLine(cate.Title);
                                if (cate.Id == categoryBlog.ParentCategoryId)
                                {
                                    canUpdate = false;
                                    ModelState.AddModelError(string.Empty, "Phai chon danh muc cha khac");
                                    return true;
                                }
                                if (cate.CategoryChildren != null)
                                    return checkCateIds(cate.CategoryChildren.ToList());
                            }
                            return false;
                        };
                checkCateIds(childCates.ToList());
            }    

            if (ModelState.IsValid && canUpdate)
            {
                try
                {
                    if (categoryBlog.ParentCategoryId == -1) categoryBlog.ParentCategoryId = null;

                    var dtc = _context.CategoryBlogs.FirstOrDefault(c => c.Id == id);
                    _context.Entry(dtc).State = EntityState.Detached;

                    _context.Update(categoryBlog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryBlogExists(categoryBlog.Id))
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

            var qr = (from c in _context.CategoryBlogs select c)
                    .Include(c => c.ParentCategory)
                    .Include(c => c.CategoryChildren);

            var categories = (await qr.ToListAsync())
                            .Where(c => c.ParentCategory == null)
                            .ToList();
            categories.Insert(0, new CategoryBlog()
            {
                Id = -1,
                Title = "Khong co danh muc cha"
            });

            var items = new List<CategoryBlog>();
            CreateSelectItems(categories, items, 0);

            var selectList = new SelectList(items, "Id", "Title");

            ViewData["ParentCategoryId"] = selectList;
            return View(categoryBlog);
        }

        // GET: admin/CategoryBlogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryBlog = await _context.CategoryBlogs
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoryBlog == null)
            {
                return NotFound();
            }

            return View(categoryBlog);
        }

        // POST: admin/CategoryBlogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoryBlog = await _context.CategoryBlogs
                               .Include(c=> c.CategoryChildren)
                               .FirstOrDefaultAsync(c => c.Id == id);

            if (categoryBlog == null)
            {
                return NotFound();
            }    

            foreach (var cCategory in categoryBlog.CategoryChildren)
            {
                cCategory.ParentCategoryId = categoryBlog.ParentCategoryId;
            }

            _context.CategoryBlogs.Remove(categoryBlog);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }

        private bool CategoryBlogExists(int id)
        {
            return _context.CategoryBlogs.Any(e => e.Id == id);
        }
    }
}
