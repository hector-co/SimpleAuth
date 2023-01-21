namespace SimpleAuth.Domain.Model;

public partial class ServerSettings
{
#nullable disable
    private ServerSettings() { }
#nullable enable

    public ServerSettings(bool allowSelfRegistration)
    {
        AllowSelfRegistration = allowSelfRegistration;
    }

    public int Id { get; private set; }
    public bool AllowSelfRegistration { get; set; }
}
