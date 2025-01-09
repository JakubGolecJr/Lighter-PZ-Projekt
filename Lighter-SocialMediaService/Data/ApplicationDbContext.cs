using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Lighter_SocialMediaService.Models;

namespace Lighter_SocialMediaService.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Like> Likes { get; set; }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Wymuś zapis dat w UTC
            builder.Entity<Post>()
                .Property(p => p.CreatedAt)
                .HasConversion(
                    v => v.ToUniversalTime(), // Konwersja do UTC przy zapisie
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)); // Ustawienie UTC przy odczycie
        }
    }
}
