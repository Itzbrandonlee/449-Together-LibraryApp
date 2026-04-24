using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Api.Dtos;
using LibraryManagement.Api.Services;

namespace LibraryManagement.Api.Controllers;

[ApiController]
[Route("api/borrowing")]
public class BorrowingController : ControllerBase
{
    private readonly IBorrowRecordService _borrowService;

    public BorrowingController(IBorrowRecordService borrowService)
    {
        _borrowService = borrowService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BorrowRecordResponse>>> GetBorrowRecords()
    {
        var records = await _borrowService.GetBorrowRecordsAsync();
        return Ok(records);
    }

    [HttpGet("history/{memberId:guid}")]
    public async Task<ActionResult<IEnumerable<BorrowRecordResponse>>> GetMemberHistory(Guid memberId)
    {
        var history = await _borrowService.GetBorrowRecordsByMemberIdAsync(memberId);
        return Ok(history);
    }

    [HttpPost("borrow")]
    public async Task<ActionResult<BorrowRecordResponse>> BorrowBook([FromBody] CreateBorrowRequest request)
    {
        if (request.BookId == Guid.Empty || request.MemberId == Guid.Empty)
            return BadRequest(new { error = "BookId and MemberId are strictly required." });

        var result = await _borrowService.BorrowBookAsync(request);
        return Ok(result);
    }

    [HttpPost("return/{id:guid}")]
    public async Task<ActionResult<BorrowRecordResponse>> ReturnBook(Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest(new { error = "A valid BorrowRecord ID is required." });

        var result = await _borrowService.ReturnBookAsync(id);
        if (result is null)
            return NotFound(new { error = $"Borrow record with ID {id} not found." });

        return Ok(result);
    }
}