using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using SimpleAuth.Domain.Model;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Roles;

internal static class RoleQueryableExtensions
{
    internal static IQueryable<Role> AddIncludes
        (this IQueryable<Role> queryable)
    {
        return queryable
            .Include(m => m.Claims)
            ;
    }
}