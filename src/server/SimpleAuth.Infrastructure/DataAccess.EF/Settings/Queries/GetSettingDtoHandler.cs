using Mapster;
using Microsoft.EntityFrameworkCore;
using SimpleAuth.Domain.Model;
using SimpleAuth.Application.Common.Queries;
using SimpleAuth.Application.Settings.Queries;

namespace SimpleAuth.Infrastructure.DataAccess.EF.Settings.Queries;

public class GetSettingDtoHandler : IQueryHandler<GetSettingDto, SettingDto>
{
    private readonly SimpleAuthContext _context;

    public GetSettingDtoHandler(SimpleAuthContext context)
    {
        _context = context;
    }

    public async Task<Result<SettingDto>> Handle(GetSettingDto request, CancellationToken cancellationToken)
    {
        var data = await _context.Set<Setting>()
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        return new Result<SettingDto>(data?.Adapt<SettingDto>());
    }
}