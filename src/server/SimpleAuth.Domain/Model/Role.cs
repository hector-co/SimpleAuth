using Microsoft.AspNetCore.Identity;

namespace SimpleAuth.Domain.Model;

public partial class Role : IdentityRole<Guid>
{
    public Role()
    {
        Id = Guid.NewGuid();
        Claims = new List<RoleClaim>();
    }

    public List<RoleClaim> Claims { get; set; }
    public bool AssignByDefault { get; set; }
}
