using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text.Json;

#nullable disable

namespace SimpleAuth.Domain.Model;

public partial class Role
{
    internal List<User> UserRoles { get; set; } = new List<User>();
}

public partial class User
{
    internal List<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

public partial class UserRole : IdentityUserRole<Guid>
{
    public User User { get; set; }
    public Role Role { get; set; }
}
