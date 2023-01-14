using SimpleAuth.Application.Common.Commands;

namespace SimpleAuth.Application.Server.Commands;

public record SetupServer
(
    List<SetupServer.SetupRole> Roles,
    List<SetupServer.SetupScopes> Scopes,
    List<SetupServer.ConfidentialApp> ConfidentialApps,
    List<SetupServer.PublicApp> PublicApps
) : ICommand
{
    public record SetupRole(string Name, bool AssignByDefault);
    public record SetupScopes(string Name, string DisplayName);
    public record ConfidentialApp(string Id, string Secret, string DisplayName, List<string> Scopes);
    public record PublicApp(string Id, string DisplayName, List<string> RedirectUris,
        List<string> PostLogoutRedirectUris, List<string> Scopes);
}
