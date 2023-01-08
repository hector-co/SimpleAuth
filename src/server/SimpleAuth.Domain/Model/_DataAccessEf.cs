using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text.Json;

#nullable disable

namespace SimpleAuth.Domain.Model;

public partial class Role
{
    internal List<User> UserRoles { get; set; }
}
