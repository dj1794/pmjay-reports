namespace Pmjay.Api.Data;

public class Agra1Dto
{
 public string? src_family_id { get; set; }
 public string? src_member_id { get; set; }
 public string? name { get; set; }
 public string? relation { get; set; }
 public string? father_guardian_name { get; set; }
 public string? rural_urban_flag { get; set; }
 public string? district_name { get; set; }
 public string? blockname { get; set; }
 public string? source_address { get; set; }
 public string? addressasperadhaarofapproved { get; set; }
 public string? village_ward_lgd_code { get; set; }
 public string? source_type { get; set; }
 public string? card_status_member { get; set; }
 public string? card_status_family { get; set; }
 public string? memberbelongtozeropovery { get; set; }
}

public class VerificationDetail
{
    public long Id { get; set; }
    public string MemberId { get; set; }
    public string FamilyId { get; set; }
    public string? Remarks { get; set; }
    public int Status { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}