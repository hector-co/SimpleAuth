using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleAuth.Domain.Model;
using SimpleAuth.Infrastructure.DataAccess.EF.Roles;
using SimpleAuth.Infrastructure.DataAccess.EF.Users;

namespace SimpleAuth.Infrastructure.DataAccess.EF;

public class SimpleAuthContext
    : IdentityDbContext<User, Role, int, UserClaim, IdentityUserRole<int>, IdentityUserLogin<int>, RoleClaim, IdentityUserToken<int>>
{
    public const string DbSchema = "auth";

    public SimpleAuthContext(DbContextOptions<SimpleAuthContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Configure(modelBuilder);
    }

    public static void Configure(ModelBuilder modelBuilder, string dbSchema = DbSchema)
    {
        modelBuilder.ApplyConfiguration(new RoleConfiguration(dbSchema));
        modelBuilder.ApplyConfiguration(new RoleClaimConfiguration(dbSchema));
        modelBuilder.ApplyConfiguration(new UserConfiguration(dbSchema));
        modelBuilder.ApplyConfiguration(new UserClaimConfiguration(dbSchema));

        modelBuilder.Entity<IdentityUserLogin<int>>(b =>
        {
            b.HasKey(l => new { l.LoginProvider, l.ProviderKey });

            b.Property(l => l.LoginProvider).HasMaxLength(128);
            b.Property(l => l.ProviderKey).HasMaxLength(128);

            b.ToTable("UserLogins", dbSchema);
        });

        modelBuilder.Entity<IdentityUserToken<int>>(b =>
        {
            b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

            b.Property(t => t.LoginProvider).HasMaxLength(128);
            b.Property(t => t.Name).HasMaxLength(128);

            b.ToTable("UserTokens", dbSchema);
        });
    }
}