namespace SimpleAuth.Domain.Model;

public partial class Setting
{
#nullable disable
    private Setting() { }
#nullable enable

    public Setting(bool allowSelfRegistration)
    {
        AllowSelfRegistration = allowSelfRegistration;
    }

    public int Id { get; private set; }
    public bool AllowSelfRegistration { get; set; }
}
