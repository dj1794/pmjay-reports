namespace Pmjay.Web.Data
{
    public class VerificationDetailRequestDto
    {
        public string MemberId { get; set; }
        public string FamilyId { get; set; }
        public string Remarks { get; set; }
        public int Status { get; set; }
    }
    public class VerificationDetail
    {
        public long Id { get; set; }
        public string MemberId { get; set; }
        public string FamilyId { get; set; }
        public string Remarks { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
