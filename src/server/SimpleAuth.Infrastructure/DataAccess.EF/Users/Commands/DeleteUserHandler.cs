using Microsoft.EntityFrameworkCore;
using SimpleAuth.Domain.Model;
using SimpleAuth.Domain.Abstractions;
using SimpleAuth.Application.Abstractions.Commands;
using SimpleAuth.Application.Commands.Users;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Users.Commands;

public class DeleteUserHandler : ICommandHandler<DeleteUser>
{
    private readonly SimpleAuthContext _context;

    public DeleteUserHandler(SimpleAuthContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(DeleteUser request, CancellationToken cancellationToken)
    {
        var user = await _context.Set<User>()
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (user == null)
            return Response.Failure(new Error("User.Delete.NotFound", "Entity not found."));

        _context.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
        return Response.Success();
    }
}