using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Api.Dtos;
using LibraryManagement.Api.Services;

namespace LibraryManagement.Api.Controllers;

[ApiController]
[Route("api/members")]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<MemberResponse>> GetMembers()
    {
        var members = _memberService.GetMembers();
        return Ok(members);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<MemberResponse> GetMemberById(Guid id)
    {
        var member = _memberService.GetMemberById(id);

        if (member == null)
        {
            return NotFound(new { error = $"Member with ID {id} was not found." });
        }

        return Ok(member);
    }

    [HttpPost]
    public ActionResult<MemberResponse> CreateMember([FromBody] CreateMemberRequest request)
    {

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return BadRequest(new { error = "FullName is required." });
        }

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
        {
            return BadRequest(new { error = "A valid Email is required." });
        }

        var created = _memberService.CreateMember(request);
        return CreatedAtAction(nameof(GetMemberById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public ActionResult<MemberResponse> UpdateMember(Guid id, [FromBody] UpdateMemberRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return BadRequest(new { error = "FullName is required." });
        }

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
        {
            return BadRequest(new { error = "A valid Email is required." });
        }
        var updated = _memberService.UpdateMember(id, request);

        if (updated == null)
        {
            return NotFound(new { error = $"Cannot update. Member with ID {id} not found." });
        }

        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteMember(Guid id)
    {
        var success = _memberService.DeleteMember(id);

        if (!success)
        {
            return NotFound(new { error = $"Cannot delete. Member with ID {id} not found." });
        }

        return NoContent();
    }
}