using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SimpleAuth.Domain.Model;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Users
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        private readonly string _dbSchema;

        public UserRoleConfiguration(string dbSchema)
        {
            _dbSchema = dbSchema;
        }

        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasKey(m => new { m.RoleId, m.UserId });
            builder.ToTable("UserRoles", _dbSchema);
            builder.HasOne(m => m.User).WithMany(m => m.UserRoles)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(m => m.Role)
                .WithMany().OnDelete(DeleteBehavior.Restrict);
        }
    }
}
