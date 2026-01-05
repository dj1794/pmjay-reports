using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Pmjay.Api.Data.AgraDataService;

namespace Pmjay.Api.Data;

public class CurrentUserService : ICurrentUserService
{
    public int UserId { get; }

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var userId = httpContextAccessor.HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;

        UserId = string.IsNullOrEmpty(userId) ? 0 : int.Parse(userId);
    }
}
public class AgraDataService
{
    private readonly AgraDbContext _db;

    public AgraDataService(AgraDbContext db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }
    public async Task<int> GetTotalAsync()
    {
        return await _db.Agra1.CountAsync();
    }

    // Server-side filtering and paging
    public async Task<List<Dictionary<string, object?>>> GetPageAsync(int page, int pageSize,
        string? block = null, string? village = null, string? ru = null, string? sourceType = null,
        string? memberStatus = null, string? familyStatus = null, string? search = null)
    {
        int offset = (page -1) * pageSize;

        IQueryable<Agra1Dto> q = _db.Agra1.AsNoTracking();

        q = ApplyFilters(q, block, village, ru, sourceType, memberStatus, familyStatus, search);

        var list = await q.Skip(offset).Take(pageSize).ToListAsync();

        var results = new List<Dictionary<string, object?>>();
        foreach (var dto in list)
        {
            var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(dto.src_family_id)] = dto.src_family_id,
                [nameof(dto.src_member_id)] = dto.src_member_id,
                [nameof(dto.name)] = dto.name,
                [nameof(dto.relation)] = dto.relation,
                [nameof(dto.father_guardian_name)] = dto.father_guardian_name,
                [nameof(dto.rural_urban_flag)] = dto.rural_urban_flag,
                [nameof(dto.district_name)] = dto.district_name,
                [nameof(dto.blockname)] = dto.blockname,
                [nameof(dto.source_address)] = dto.source_address,
                [nameof(dto.addressasperadhaarofapproved)] = dto.addressasperadhaarofapproved,
                [nameof(dto.village_ward_lgd_code)] = dto.village_ward_lgd_code,
                [nameof(dto.source_type)] = dto.source_type,
                [nameof(dto.card_status_member)] = dto.card_status_member,
                [nameof(dto.card_status_family)] = dto.card_status_family,
                [nameof(dto.memberbelongtozeropovery)] = dto.memberbelongtozeropovery,
            };
            results.Add(row);
        }

        return results;
    }

    // EF Core method to fetch typed DTOs (with filters)
    public async Task<List<Agra1Dto>> GetSelectedColumnsAsync(int page, int pageSize,
        string? block = null, string? village = null, string? ru = null, string? sourceType = null,
        string? memberStatus = null, string? familyStatus = null, string? search = null)
    {
        IQueryable<Agra1Dto> q = _db.Agra1.AsNoTracking();
        q = ApplyFilters(q, block, village, ru, sourceType, memberStatus, familyStatus, search);
        return await q.Skip((page -1) * pageSize).Take(pageSize).ToListAsync();
    }

    // New: compute summary totals from the database with filters
    public async Task<SummaryDto> GetFilteredSummaryAsync(string? block = null, string? village = null, string? ru = null, string? sourceType = null,
        string? memberStatus = null, string? familyStatus = null, string? search = null)
    {
        IQueryable<Agra1Dto> q = _db.Agra1.AsNoTracking();
        q = ApplyFilters(q, block, village, ru, sourceType, memberStatus, familyStatus, search);

        // Total members = total rows matching filters
        var totalMembers = await q.CountAsync();

        // Total families = distinct non-empty src_family_id
        var totalFamilies = await q.Where(a => !string.IsNullOrEmpty(a.src_family_id))
            .Select(a => a.src_family_id)
            .Distinct()
            .CountAsync();

        // Covered members = count where card_status_member == 'Approved' (case-insensitive)
        var coveredMembers = await q.Where(a => !string.IsNullOrEmpty(a.card_status_member) && a.card_status_member == "Approved").CountAsync();

        // Covered families = distinct src_family_id where any member in family has card_status_member == 'Approved'
        var coveredFamilies = await q.Where(a => !string.IsNullOrEmpty(a.src_family_id) && !string.IsNullOrEmpty(a.card_status_member) && a.card_status_member == "Approved")
            .Select(a => a.src_family_id)
            .Distinct()
            .CountAsync();

        return new SummaryDto
        {
            TotalMembers = totalMembers,
            TotalFamilies = totalFamilies,
            CoveredMembers = coveredMembers,
            CoveredFamilies = coveredFamilies
        };
    }

    private static IQueryable<Agra1Dto> ApplyFilters(
        IQueryable<Agra1Dto> q,
        string? block, string? village, string? ru, string? sourceType,
        string? memberStatus, string? familyStatus, string? search)
    {
        if (!string.IsNullOrWhiteSpace(block))
        {
            var b = block.Trim();
            q = q.Where(a => a.blockname != null && a.blockname == b);
        }

        if (!string.IsNullOrWhiteSpace(village))
        {
            var v = village.Trim();
            q = q.Where(a =>
                a.source_address != null &&
                EF.Functions.Like(a.source_address, $"%{v}%"));
        }

        if (!string.IsNullOrWhiteSpace(ru))
        {
            var r = ru.Trim();
            q = q.Where(a => a.rural_urban_flag != null && a.rural_urban_flag == r);
        }

        if (!string.IsNullOrWhiteSpace(sourceType))
        {
            var s = sourceType.Trim();
            q = q.Where(a => a.source_type != null && a.source_type == s);
        }

        if (!string.IsNullOrWhiteSpace(memberStatus))
        {
            q = memberStatus switch
            {
                "Approved" => q.Where(a =>
                    a.card_status_member != null &&
                    a.card_status_member.ToUpper() == "Approved"),

                "Applied" => q.Where(a =>
                    a.card_status_member == "S"),

                "Disable" => q.Where(a =>
                    a.card_status_member != null &&
                    a.card_status_member.ToUpper()=="Disable"),

                "EKYC" => q.Where(a =>
                    a.card_status_member == null ||
                    a.card_status_member == "DO Ekyc"),

                _ => q
            };
        }
        if (!string.IsNullOrWhiteSpace(familyStatus))
        {
            var f = familyStatus.Trim();
            q = q.Where(a => a.card_status_family != null && a.card_status_family == f);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            q = q.Where(a =>
                (a.name != null && EF.Functions.Like(a.name, $"%{s}%")) ||
                (a.src_family_id != null && EF.Functions.Like(a.src_family_id, $"%{s}%"))
            );
        }

        return q;
    }
    public async Task<VerificationDetail?> GetVerificationAsync(string memberId)
    {
        return await _db.VerificationDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MemberId == memberId);
    }
    public async Task<List<VerificationDetail>> GetVerificationsByFamilyAsync(string familyId)
    {
        return await _db.VerificationDetails
            .AsNoTracking()
            .Where(x => x.FamilyId == familyId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
    public async Task<VerificationDetail> UpsertVerificationAsync(
    VerificationDetailRequestDto dto,
    int userId)
    {
        var existing = await _db.VerificationDetails
            .FirstOrDefaultAsync(x => x.MemberId == dto.MemberId);

        if (existing == null)
        {
            existing = new VerificationDetail
            {
                MemberId = dto.MemberId,
                FamilyId = dto.FamilyId,
                Remarks = dto.Remarks,
                Status = dto.Status,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            _db.VerificationDetails.Add(existing);
        }
        else
        {
            existing.Remarks = dto.Remarks;
            existing.Status = dto.Status;
            existing.UpdatedBy = userId;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        return existing;
    }


}
