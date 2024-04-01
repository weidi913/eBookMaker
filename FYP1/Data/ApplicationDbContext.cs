using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FYP1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FYP1.Data
{
    public class ApplicationDbContext : IdentityDbContext<Member>
    {


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }



        public DbSet<FYP1.Models.Chapter>? Chapter { get; set; }
        public DbSet<FYP1.Models.eBook>? eBook { get; set; }
        public DbSet<FYP1.Models.Element>? Element { get; set; }
        public DbSet<FYP1.Models.BookPage>? BookPage { get; set; }
        public DbSet<FYP1.Models.Collaboration>? Collaboration { get; set; }
        public DbSet<FYP1.Models.Member>? Member { get; set; }
        public DbSet<FYP1.Models.Version>? Version { get; set; }
        public DbSet<FYP1.Models.Comment>? Comment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Call the base implementation first

            modelBuilder.Entity<Collaboration>()
                .HasKey(c => new { c.authorID, c.bookID });
        }
    }
}