using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Identity.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, long>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}