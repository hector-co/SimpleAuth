using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Linq;
using SimpleAuth.Domain.Model;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Settings;

public class SettingConfiguration : IEntityTypeConfiguration<Setting>
{
    private readonly string _dbSchema;

    public SettingConfiguration(string dbSchema)
    {
        _dbSchema = dbSchema;
    }

    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.ToTable("Setting", _dbSchema);
    }
}