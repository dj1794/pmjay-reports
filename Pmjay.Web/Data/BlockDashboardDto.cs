namespace Pmjay.Web.Data
{
    public class BlockDashboardDto
    {   public string? BlockName { get; set; } = "";
        public int TotalFamilies { get; set; }
        public int CoveredFamilies { get; set; }
        public int TotalMembers { get; set; }
        public int CoveredMembers { get; set; }

        public int FamilyCoveragePercent => TotalFamilies == 0 
        ? 0 
        : (int)Math.Round((double)CoveredFamilies * 100 / TotalFamilies);

    public int MemberCoveragePercent => TotalMembers == 0 
        ? 0 
        : (int)Math.Round((double)CoveredMembers * 100 / TotalMembers);
    }
}
