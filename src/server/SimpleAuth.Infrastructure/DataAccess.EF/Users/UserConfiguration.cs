using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Linq;
using SimpleAuth.Domain.Model;
using Microsoft.AspNetCore.Identity;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    private readonly string _dbSchema;

    public UserConfiguration(string dbSchema)
    {
        _dbSchema = dbSchema;
    }

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", _dbSchema);
        builder.Property(m => m.UserName)
            .HasMaxLength(256);
        builder.Property(m => m.Email)
            .HasMaxLength(256);
        builder.Property(m => m.DisplayName)
            .HasMaxLength(256);
        builder.Property(m => m.Name)
            .HasMaxLength(256);
        builder.Property(m => m.LastName)
            .HasMaxLength(256);
        builder.HasMany(m => m.Claims)
            .WithOne()
            .HasForeignKey(r => r.UserId);

        builder.Ignore(m => m.Roles);

        builder.HasIndex(u => u.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
        builder.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex");
        builder.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();
        builder.Property(u => u.NormalizedUserName).HasMaxLength(256);
        builder.Property(u => u.NormalizedEmail).HasMaxLength(256);
    }
}