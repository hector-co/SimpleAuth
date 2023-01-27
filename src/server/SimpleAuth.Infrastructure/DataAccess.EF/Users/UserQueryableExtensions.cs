using Microsoft.EntityFrameworkCore;
using SimpleAuth.Application.Users.Queries;
using SimpleAuth.Domain.Model;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Users;

internal static class UserQueryableExtensions
{
    static UserQueryableExtensions()
    {
        Mapster.TypeAdapterConfig<User, UserDto>
            .NewConfig()
            .Map(dest => dest.Roles, src => src.UserRoles.Select(ur => ur.Role).ToList());

    }

    internal static IQueryable<User> AddIncludes
        (this IQueryable<User> queryable)
    {
        return queryable
            .Include(m => m.UserRoles)
                .ThenInclude(r => r.Role);
    }
}