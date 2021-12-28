using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WholeSaleManagementApp.Models;
using WholeSaleManagementApp.Models.Blog;
using WholeSaleManagementApp.Models.Contact;

namespace WholeSaleManagementApp.Data
{
    public class MyDbContext : IdentityDbContext<AppUser>
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            modelBuilder.Entity<CategoryBlog>(entity =>
            {
                entity.HasIndex(c => c.Slug)
                      .IsUnique();
            });

            //Tao khoa chinh cho bang nhieu nhieu PostCategory
            modelBuilder.Entity<PostCategory>(entity =>
            {
                entity.HasKey(c => new { c.PostID, c.CategoryID });
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasIndex(c => c.Slug)
                      .IsUnique();
            });
        }

        public DbSet<Contact> Contacts { get; set; }

        public DbSet<CategoryBlog> CategoryBlogs { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<PostCategory> PostCategories { get; set; }  
    }
}
