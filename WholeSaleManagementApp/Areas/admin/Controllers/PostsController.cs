using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using App.Data;
using App.Models;
using App.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WholeSaleManagementApp.Areas.admin.Models.Blog;
using WholeSaleManagementApp.Data;
using WholeSaleManagementApp.Models;
using WholeSaleManagementApp.Models.Blog;
using WholeSalerWeb.Helpper;

namespace WholeSaleManagementApp.Areas.admin.Controllers
{
    [Area("admin")]
    [Route("admin/blog/post/[action]/{id?}")]
    [Authorize(Roles = RoleName.Administrator+ "," + RoleName.Editor)]
    public class PostsController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        [TempData]
        public string StatusMessage { get; set; }
        public PostsController(MyDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: admin/Posts
        public async Task<IActionResult> Index([FromQuery(Name = "p")]int currentPage, int pagesize)
        {
            var posts = _context.Posts
                        .Include(p => p.Author)
                        .OrderByDescending(p => p.DateUpdated);

            int totalPosts = await posts.CountAsync();
            if (pagesize <= 0) pagesize = 10;
            int countPages = (int)Math.Ceiling((double)totalPosts / pagesize);

            if (currentPage > countPages) currentPage = countPages;
            if (currentPage < 1) currentPage = 1;

            var pagingModel = new PagingModel()
            {
                countpages = countPages,
                currentpage = currentPage,
                generateUrl = (pageNumber) => Url.Action("Index", new
                {
                    p = pageNumber,
                    pagesize = pagesize
                })
            };

            ViewBag.pagingModel  = pagingModel;
            ViewBag.totalPosts = totalPosts;

            var postsInPage = await posts.Skip((currentPage - 1) * pagesize)
                        .Take(pagesize)
                        .Include(p => p.PostCategories)
                        .ThenInclude(pc => pc.Category)
                        .ToListAsync();
            

            return View(postsInPage);
        }

        // GET: admin/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: admin/Posts/Create
        public async Task<IActionResult> CreateAsync()
        {
            var categories = await _context.CategoryBlogs.ToListAsync();

            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");

            return View();
        }

        // POST: admin/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,Thumb,Content,Published,CategoryIDs")] CreatePostModel post, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {

            var categories = await _context.CategoryBlogs.ToListAsync();

            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");

            if (post.Slug == null)
            {
                post.Slug = AppUtilities.GenerateSlug(post.Title);
            }

            if (await _context.Posts.AnyAsync(p => p.Slug == post.Slug))
            {
                ModelState.AddModelError("Slug", "Nhap chuoi Url khac");
                return View(post);
            };



            if (ModelState.IsValid)
            {
                post.Title = Ultilities.ToTitleCase(post.Title);
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Ultilities.SEOUrl(post.Title) + extension;
                    post.Thumb = await Ultilities.UploadFile(fThumb, @"posts", image.ToLower());
                }
                if (string.IsNullOrEmpty(post.Thumb)) post.Thumb = "default.jpg";
                var user = await _userManager.GetUserAsync(this.User);

                post.DateCreated = post.DateUpdated = DateTime.Now;
                post.AuthorId = user.Id;

                _context.Add(post);

                if(post.CategoryIDs !=null)
                {
                    foreach (var CateId in post.CategoryIDs)
                    {
                        _context.Add(new PostCategory()
                        {
                            CategoryID = CateId,
                            Post = post
                        });
                    }
                }

                await _context.SaveChangesAsync();

                StatusMessage = "Vua tao bai viet moi";

                return RedirectToAction(nameof(Index));
            }


            return View(post);
        }

        // GET: admin/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.PostCategories).FirstOrDefaultAsync(p => p.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            var postEdit = new CreatePostModel()
            {
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
                Description = post.Description,
                Thumb = post.Thumb,
                Slug = post.Slug,
                Published = post.Published,
                CategoryIDs = post.PostCategories.Select(pc => pc.CategoryID).ToArray()
            };

            var categories = await _context.CategoryBlogs.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");

            return View(postEdit);
        }

        // POST: admin/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Description,Slug,Thumb,Content,Published,CategoryIDs")] CreatePostModel post, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            var categories = await _context.CategoryBlogs.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");

            if (post.Slug == null)
            {
                post.Slug = AppUtilities.GenerateSlug(post.Title);
            };

            if (await _context.Posts.AnyAsync(p => p.Slug == post.Slug && p.PostId != id))
            {
                ModelState.AddModelError("Slug", "Nhap chuoi Url khac");
                return View(post);
            };

            if (ModelState.IsValid)
            {
                try
                {
                    post.Title = Ultilities.ToTitleCase(post.Title);
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string image = Ultilities.SEOUrl(post.Title) + extension;
                        post.Thumb = await Ultilities.UploadFile(fThumb, @"posts", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(post.Thumb)) post.Thumb = "default.jpg";

                    var postUpdate = await _context.Posts.Include(p => p.PostCategories).FirstOrDefaultAsync(p => p.PostId == id);
                    if (postUpdate == null)
                    {
                        return NotFound();
                    }

                    postUpdate.Title = post.Title;
                    postUpdate.Description = post.Description;
                    postUpdate.Content = post.Content;
                    postUpdate.Published = post.Published;
                    postUpdate.Slug = post.Slug;
                    postUpdate.Thumb = post.Thumb;
                    postUpdate.DateUpdated = DateTime.Now;

                    //Update PostCategory
                    if (post.CategoryIDs == null) post.CategoryIDs = new int[] { };

                    var oldCateIds = postUpdate.PostCategories.Select(c => c.CategoryID).ToArray();
                    var newCateIds = post.CategoryIDs;

                    var removeCatePosts = from postCate in postUpdate.PostCategories
                                          where (!newCateIds.Contains(postCate.CategoryID))
                                          select postCate;
                    _context.PostCategories.RemoveRange(removeCatePosts);

                    var addCateIds = from CateId in newCateIds
                                     where !oldCateIds.Contains(CateId)
                                     select CateId;

                    foreach (var CateId in addCateIds)
                    {
                        _context.PostCategories.Add(new PostCategory() {
                            PostID = id,
                            CategoryID = CateId
                        });
                    }


                    _context.Update(postUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                StatusMessage = "Vua cap nhat bai viet";

                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: admin/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: admin/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            StatusMessage = "Ban vua xoa bai viet:"
                + post.Title;

            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
