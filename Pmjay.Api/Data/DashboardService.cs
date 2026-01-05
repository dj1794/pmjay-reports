using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Pmjay.Api.Data; 

namespace Pmjay.Api.Data
{
    public class DashboardService
    {
        private readonly AgraDbContext _context;
        private readonly IMemoryCache _cache;

        public DashboardService(AgraDbContext context, IMemoryCache cache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }
        public async Task<List<BlockDashboardDto>> GetDashboardAsync()
        {
            return await _cache.GetOrCreateAsync("block-dashboard", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

                return await _context.Set<BlockDashboardDto>() // no DbSet property needed
                    .FromSqlRaw("EXEC sp_BlockDashboard")
                    .AsNoTracking()
                    .ToListAsync();
            }) ?? new List<BlockDashboardDto>(); // in case cache returns null
        }
    }
}