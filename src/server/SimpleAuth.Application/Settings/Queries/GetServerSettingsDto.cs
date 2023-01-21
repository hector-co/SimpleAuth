using SimpleAuth.Application.Common.Queries;

namespace SimpleAuth.Application.Settings.Queries;

public record GetServerSettingsDto() : IQuery<ServerSettingsDto>;