namespace Pmjay.Api.Data
{
    public class VerificationDetailRequestDto
    {
        public string MemberId { get; set; }
        public string FamilyId { get; set; }
        public int RemarkId { get; set; }
        public int Status { get; set; }
    }
    public class VerificationDetailDto
    {
        public long Id { get; set; }
        public string MemberId { get; set; }
        public string FamilyId { get; set; }
        public int RemarkId { get; set; }
        public int Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}
