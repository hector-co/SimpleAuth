using Microsoft.Extensions.Localization;
using System.Reflection;

namespace SimpleAuth.Server.Resources.Localizers;

public class SharedResourceLocalizer
{
    private readonly IStringLocalizer _localizer;

    public SharedResourceLocalizer(IStringLocalizerFactory factory)
    {
        var assembly = new AssemblyName(typeof(SharedResource).Assembly.FullName!);
        _localizer = factory.Create(nameof(SharedResource), assembly.Name!);
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
