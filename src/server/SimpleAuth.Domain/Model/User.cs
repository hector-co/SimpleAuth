using Microsoft.AspNetCore.Identity;

namespace SimpleAuth.Domain.Model;

public partial class User : IdentityUser
{
    public User()
    {
        Roles = new List<Role>();
        Claims = new List<UserClaim>();
    }

    public List<Role> Roles { get; set; }
    public List<UserClaim> Claims { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
