using Microsoft.Extensions.Localization;
using System.Reflection;

namespace SimpleAuth.Server.Resources.Localizers;

public class EmailResourceLocalizer
{
    private readonly IStringLocalizer _localizer;

    public EmailResourceLocalizer(IStringLocalizerFactory factory)
    {
        var assembly = new AssemblyName(typeof(EmailResource).Assembly.FullName!);
        _localizer = factory.Create(nameof(EmailResource), assembly.Name!);
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
