using Mapster;
using Microsoft.EntityFrameworkCore;
using SimpleAuth.Domain.Model;
using SimpleAuth.Application.Common.Queries;
using SimpleAuth.Application.Settings.Queries;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Settings.Queries;

public class GetServerSettingsDtoHandler : IQueryHandler<GetServerSettingsDto, ServerSettingsDto>
{
    private readonly SimpleAuthContext _context;

    public GetServerSettingsDtoHandler(SimpleAuthContext context)
    {
        _context = context;
    }

    public async Task<Result<ServerSettingsDto>> Handle(GetServerSettingsDto request, CancellationToken cancellationToken)
    {
        var data = await _context.Set<ServerSettings>()
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        return new Result<ServerSettingsDto>(data?.Adapt<ServerSettingsDto>());
    }
}