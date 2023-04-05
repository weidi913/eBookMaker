using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FYP1.Models;

namespace FYP1.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<FYP1.Models.Chapter>? Chapter { get; set; }
        public DbSet<FYP1.Models.eBook>? eBook { get; set; }
        public DbSet<FYP1.Models.Element>? Element { get; set; }
        public DbSet<FYP1.Models.BookPage>? Page { get; set; }
    }
}