using LibraryManagement.Api.Models;

namespace LibraryManagement.Api.Repositories;

public interface IBorrowRecordRepository
{
    Task<IEnumerable<BorrowRecord>> GetAllAsync();
    Task<BorrowRecord?> GetByIdAsync(Guid id);
    Task<IEnumerable<BorrowRecord>> GetByMemberIdAsync(Guid memberId);
    Task<BorrowRecord> AddAsync(BorrowRecord record);
    Task<BorrowRecord> UpdateAsync(BorrowRecord record);
    Task<bool> HasActiveBorrowAsync(Guid memberId, Guid bookId);
}