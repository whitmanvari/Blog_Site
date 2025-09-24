using Microsoft.EntityFrameworkCore;
using Blog_Site.Models.Concrete;

namespace Blog_Site.Data
{
    public class BlogContext(DbContextOptions<BlogContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // For PostTag we arrange keys. [1, 2]
            modelBuilder.Entity<PostTag>()
             .HasKey(pt => new { pt.PostId, pt.TagId });

            modelBuilder.Entity<PostTag>()
             .HasOne(pt => pt.Post)
             .WithMany(p => p.PostTags)
             .HasForeignKey(pt => pt.PostId);

            modelBuilder.Entity<PostTag>()
             .HasOne(pt => pt.Tag)
             .WithMany(t => t.PostTags)
             .HasForeignKey(pt => pt.TagId);

            modelBuilder.Entity<Comment>()
             .HasOne(c => c.User)
             .WithMany(u => u.Comments)
             .HasForeignKey(c => c.UserId)
             .OnDelete(DeleteBehavior.Restrict);
        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<Admin> Admin { get; set; }
    }
}