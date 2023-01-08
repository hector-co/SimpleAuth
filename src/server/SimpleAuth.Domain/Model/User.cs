using Microsoft.AspNetCore.Identity;

namespace SimpleAuth.Domain.Model;

public partial class User : IdentityUser<int>
{
    public User()
    {
        Roles = new List<Role>();
        Claims = new List<UserClaim>();
    }
    public bool IsAdmin { get; set; }
    public List<Role> Roles { get; set; }
    public List<UserClaim> Claims { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool Disabled { get; set; }
}
