using SimpleAuth.Application.Common.Queries;

namespace SimpleAuth.Application.Settings.Queries;

public record GetSettingDto() : IQuery<SettingDto>;