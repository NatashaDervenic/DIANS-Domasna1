using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using webSchool.Models;

namespace webSchool.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<School> schools { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}