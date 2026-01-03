using Microsoft.AspNetCore.Mvc;
using Pmjay.Api.Data;

namespace Pmjay.Api.Controllers
{
    [ApiController]
    [Route("api/verification")]
    public class VerificationController : ControllerBase
    {
        private readonly AgraDataService _service;

        public VerificationController(AgraDataService service)
        {
            _service = service;
        }
        [HttpGet("member/{memberId}")]
        public async Task<ActionResult<VerificationDetailDto>> GetByMember(string memberId)
        {
            var data = await _service.GetVerificationAsync(memberId);

            if (data == null)
                return NotFound();

            return Ok(new VerificationDetailDto
            {
                Id = data.Id,
                MemberId = data.MemberId,
                FamilyId = data.FamilyId,
                RemarkId = data.RemarkId,
                Status = data.Status,
                CreatedAt = data.CreatedAt,
                UpdatedAt = data.UpdatedAt
            });
        }
        [HttpGet("family/{familyId}")]
        public async Task<ActionResult<List<VerificationDetailDto>>> GetByFamily(string familyId)
        {
            var list = await _service.GetVerificationsByFamilyAsync(familyId);

            return Ok(list.Select(x => new VerificationDetailDto
            {
                Id = x.Id,
                MemberId = x.MemberId,
                FamilyId = x.FamilyId,
                RemarkId = x.RemarkId,
                Status = x.Status,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }));
        }
        [HttpPost]
        public async Task<ActionResult> Upsert(
            [FromBody] VerificationDetailRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = 1; // TODO: replace with logged-in user

            var result = await _service.UpsertVerificationAsync(dto, userId);

            return Ok(new
            {
                result.Id,
                Message = "Verification saved successfully"
            });
        }

    }
}
