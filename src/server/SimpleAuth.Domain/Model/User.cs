using Microsoft.AspNetCore.Identity;

namespace SimpleAuth.Domain.Model;

public partial class User : IdentityUser<Guid>
{
    public User()
    {
        Id = Guid.NewGuid();
        Roles = new List<Role>();
        Claims = new List<UserClaim>();
    }

    public List<Role> Roles { get; set; }
    public List<UserClaim> Claims { get; set; }
    [PersonalData]
    public string Name { get; set; } = string.Empty;
    [PersonalData]
    public string LastName { get; set; } = string.Empty;
}
