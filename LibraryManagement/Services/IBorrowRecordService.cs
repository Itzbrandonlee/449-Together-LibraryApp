using LibraryManagement.Api.Dtos;

namespace LibraryManagement.Api.Services;

public interface IBorrowRecordService
{
    IEnumerable<BorrowRecordResponse> GetBorrowRecords();
    IEnumerable<BorrowRecordResponse> GetBorrowRecordsByMemberId(Guid memberId);
    Task<BorrowRecordResponse> BorrowBook(CreateBorrowRequest request);
    Task<BorrowRecordResponse> ReturnBook(Guid borrowRecordId);
}