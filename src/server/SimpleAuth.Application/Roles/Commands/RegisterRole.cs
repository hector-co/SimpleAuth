using SimpleAuth.Application.Common.Commands;

namespace SimpleAuth.Application.Roles.Commands;

public record RegisterRole
(
    string Name,
    bool AssignByDefault
) : ICommand<string>;
