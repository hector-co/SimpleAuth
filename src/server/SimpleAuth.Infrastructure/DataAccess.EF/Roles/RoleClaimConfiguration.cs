using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Linq;
using SimpleAuth.Domain.Model;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Roles;

public class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
{
    private readonly string _dbSchema;

    public RoleClaimConfiguration(string dbSchema)
    {
        _dbSchema = dbSchema;
    }

    public void Configure(EntityTypeBuilder<RoleClaim> builder)
    {
        builder.ToTable("RoleClaims", _dbSchema);
    }
}