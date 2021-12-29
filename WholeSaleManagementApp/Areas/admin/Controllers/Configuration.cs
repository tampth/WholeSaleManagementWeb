﻿using App.Data;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WholeSaleManagementApp.Data;
using WholeSaleManagementApp.Models;
using WholeSaleManagementApp.Models.Blog;

namespace WholeSaleManagementApp.Areas.admin.Controllers
{
    [Area("admin")]
    public class Configuration : Controller
    {
        private readonly MyDbContext _dbContext;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly UserManager<AppUser> _userManager;

        public Configuration(MyDbContext dbContext, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
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

        public async Task<IActionResult> SeedDataAsync()
        {
            var rolenames = typeof(RoleName).GetFields().ToList();
            foreach (var r in rolenames)
            {
                var rolename = (string)r.GetRawConstantValue();
                var rfound = await _roleManager.FindByNameAsync(rolename);
                if (rfound == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(rolename));
                }
            }

            var useradmin = await _userManager.FindByNameAsync("admin");
            if (useradmin == null)
            {
                useradmin = new AppUser()
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true,
                };

                await _userManager.CreateAsync(useradmin, "admin123");
                await _userManager.AddToRoleAsync(useradmin, RoleName.Administrator);
            }

            SeedPostCategory();

            StatusMessage = "Vua seed Database";
            return RedirectToAction("Index");
        }

        private void SeedPostCategory()
        {
            _dbContext.CategoryBlogs.RemoveRange(_dbContext.CategoryBlogs.Where(c => c.Content.Contains("fakeData")));
            _dbContext.Posts.RemoveRange(_dbContext.Posts.Where(c => c.Content.Contains("fakeData")));

            var fakerCategory = new Faker<CategoryBlog>();
            int cm = 1;
            fakerCategory.RuleFor(c => c.Title, fk => $"CM{cm++}" + fk.Lorem.Sentence(1, 2).Trim('.'));
            fakerCategory.RuleFor(c => c.Content, fk => fk.Lorem.Sentence(5) + "[fakeData]");
            fakerCategory.RuleFor(c => c.Slug, fk => fk.Lorem.Slug());


            var cate1 = fakerCategory.Generate();
            var cate11 = fakerCategory.Generate();
            var cate12 = fakerCategory.Generate();
            var cate2 = fakerCategory.Generate();
            var cate21 = fakerCategory.Generate();
            var cate211 = fakerCategory.Generate();


            cate11.ParentCategory = cate1;
            cate12.ParentCategory = cate1;
            cate21.ParentCategory = cate2;
            cate211.ParentCategory = cate21;
            var categories = new CategoryBlog[] { cate1, cate11, cate12, cate2, cate21, cate211 };
            _dbContext.CategoryBlogs.AddRange(categories);

            //Post
            var rCateIndex = new Random();
            int bv = 1;

            var user = _userManager.GetUserAsync(this.User).Result;
            var fakerPost = new Faker<Post>();
            fakerPost.RuleFor(p => p.AuthorId, f => user.Id);
            fakerPost.RuleFor(p => p.Content, f => f.Lorem.Paragraphs(7) + "[fakeData]");
            fakerPost.RuleFor(p => p.DateCreated, f => f.Date.Between(new DateTime(2021, 1, 1), new DateTime(2021,7,1)));
            fakerPost.RuleFor(p => p.Description, f => f.Lorem.Sentences(3));
            fakerPost.RuleFor(p => p.Published, f => true);
            fakerPost.RuleFor(p => p.Slug, f => f.Lorem.Slug());
            fakerPost.RuleFor(p => p.Title, f => $"Bai {bv++}" + f.Lorem.Sentence(3,4).Trim('.'));

            List<Post> posts = new List<Post>();
            List<PostCategory> postCategories = new List<PostCategory>();

            for (int i = 0; i<40; i++)
            {
                var post = fakerPost.Generate();
                post.DateUpdated = post.DateCreated;
                posts.Add(post);
                postCategories.Add(new PostCategory()
                {
                    Post = post,
                    Category = categories[rCateIndex.Next(5)]
                }) ;
            }

            _dbContext.AddRange(posts);
            _dbContext.AddRange(postCategories);

            _dbContext.SaveChanges();
        }
    }
}