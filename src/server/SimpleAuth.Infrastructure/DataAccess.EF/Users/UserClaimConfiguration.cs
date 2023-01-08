using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Linq;
using SimpleAuth.Domain.Model;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Users;

public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
{
    private readonly string _dbSchema;

    public UserClaimConfiguration(string dbSchema)
    {
        _dbSchema = dbSchema;
    }

    public void Configure(EntityTypeBuilder<UserClaim> builder)
    {
        builder.ToTable("UserClaims", _dbSchema);
    }
}