namespace SimpleAuth.Application.Settings.Queries;

public record ServerSettingsDto(
    int Id,
    bool AllowSelfRegistration);
