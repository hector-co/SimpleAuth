using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SimpleAuth.Infrastructure.DataAccess.EF;

internal class SimpleAuthContextFactory : IDesignTimeDbContextFactory<SimpleAuthContext>
{
    public SimpleAuthContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SimpleAuthContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Database=SimpleAuth;Username=postgres;Password=postgres",
            o => o.MigrationsHistoryTable("__EFMigrationsHistory", SimpleAuthContext.DbSchema));

        return new SimpleAuthContext(optionsBuilder.Options);
    }
}