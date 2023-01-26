namespace SimpleAuth.Application.Server.Commands;

public record ServerSetup
(
    bool AllowSelfRegistration,
    List<ServerSetup.SetupRole> Roles,
    List<ServerSetup.SetupScopes> Scopes,
    List<ServerSetup.ConfidentialApp> ConfidentialApps,
    List<ServerSetup.PublicApp> PublicApps
)
{
    public record SetupRole(string Name, bool AssignByDefault);
    public record SetupScopes(string Name, string DisplayName);
    public record ConfidentialApp(string Id, string Secret, string DisplayName, List<string> Scopes);
    public record PublicApp(string Id, string DisplayName, List<string> RedirectUris,
        List<string> PostLogoutRedirectUris, List<string> Scopes);
}
