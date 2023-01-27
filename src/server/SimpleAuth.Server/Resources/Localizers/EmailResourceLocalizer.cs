using Microsoft.Extensions.Localization;
using System.Reflection;

namespace SimpleAuth.Server.Resources.Localizers;

public class EmailResourceLocalizer
{
    private readonly IStringLocalizer<EmailResource> _localizer;

    public EmailResourceLocalizer(IStringLocalizer<EmailResource> localizer)
    {
        _localizer = localizer;
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
