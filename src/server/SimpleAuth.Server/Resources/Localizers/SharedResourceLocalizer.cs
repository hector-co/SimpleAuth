using Microsoft.Extensions.Localization;

namespace SimpleAuth.Server.Resources.Localizers;

public class SharedResourceLocalizer
{
    private readonly IStringLocalizer<SharedResource> _localizer;

    public SharedResourceLocalizer(IStringLocalizer<SharedResource> factory)
    {
        _localizer = factory;
    }

    public string this[string index]
    {
        get { return _localizer[index]; }
    }

    public string this[string index, params object[] arguments]
    {
        get { return _localizer[index, arguments]; }
    }
}
