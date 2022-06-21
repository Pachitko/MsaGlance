using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Identity.Domain.Entities;

namespace Identity.DAL.Data
{
    public class AuthDbContext : IdentityDbContext<AppUser, AppRole, long>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // postgres style
            builder.Entity<AppUser>(entity =>
            {
                entity.ToTable(name: "users");
            });
            builder.Entity<IdentityUserClaim<long>>(entity =>
            {
                entity.ToTable("user_claims");
            });
            builder.Entity<AppRole>(entity =>
            {
                entity.ToTable(name: "roles");
            });
            builder.Entity<IdentityRoleClaim<long>>(entity =>
            {
                entity.ToTable("role_claims");
            });
            builder.Entity<IdentityUserRole<long>>(entity =>
            {
                entity.ToTable("user_roles");
            });
            builder.Entity<IdentityUserLogin<long>>(entity =>
            {
                entity.ToTable("user_logins");
            });
            builder.Entity<IdentityUserToken<long>>(entity =>
            {
                entity.ToTable("user_tokens");
            });
        }
    }
}