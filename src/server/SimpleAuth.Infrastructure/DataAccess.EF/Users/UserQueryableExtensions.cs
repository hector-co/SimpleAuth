using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using SimpleAuth.Domain.Model;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Users;

internal static class UserQueryableExtensions
{
    internal static IQueryable<User> AddIncludes
        (this IQueryable<User> queryable)
    {
        return queryable
            .Include(m => m.Roles)
            .Include(m => m.Claims)
            ;
    }
}