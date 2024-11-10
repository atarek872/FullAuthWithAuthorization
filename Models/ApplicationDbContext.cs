using DynamicRoleBasedAuthorization.Models.MetaVideos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DynamicRoleBasedAuthorization.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Bouns> Bouns { get; set; }
        public DbSet<MetaVids> MetaVids { get; set; }
        public DbSet<MyVideos> MyVideos { get; set; }
        public DbSet<WatchedVideos> WatchedVideos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
