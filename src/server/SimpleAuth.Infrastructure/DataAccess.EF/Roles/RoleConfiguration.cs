using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Linq;
using SimpleAuth.Domain.Model;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Roles;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    private readonly string _dbSchema;

    public RoleConfiguration(string dbSchema)
    {
        _dbSchema = dbSchema;
    }

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles", _dbSchema);
        builder.Property(m => m.Name)
            .HasMaxLength(256);
        builder.HasMany(m => m.Claims)
            .WithOne()
            .HasForeignKey(r => r.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();
        builder.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();
        builder.Property(u => u.NormalizedName).HasMaxLength(256);
    }
}