using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Linq;
using SimpleAuth.Domain.Model;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Settings;

public class ServerSettingsConfiguration : IEntityTypeConfiguration<ServerSettings>
{
    private readonly string _dbSchema;

    public ServerSettingsConfiguration(string dbSchema)
    {
        _dbSchema = dbSchema;
    }

    public void Configure(EntityTypeBuilder<ServerSettings> builder)
    {
        builder.ToTable("ServerSettings", _dbSchema);
    }
}