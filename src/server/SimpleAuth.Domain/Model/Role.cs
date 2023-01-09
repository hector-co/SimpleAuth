using Microsoft.AspNetCore.Identity;

namespace SimpleAuth.Domain.Model;

public partial class Role : IdentityRole<int>
{
    public Role()
    {
        Claims = new List<RoleClaim>();
    }

    public List<RoleClaim> Claims { get; set; }
    public bool AssignByDefault { get; set; }
    public bool Disabled { get; set; }
}
