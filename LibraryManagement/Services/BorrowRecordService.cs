using LibraryManagement.Api.Dtos;
using LibraryManagement.Api.Models;
using LibraryManagement.Api.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace LibraryManagement.Api.Services;

public class BorrowRecordService : IBorrowRecordService
{
    private readonly IBorrowRecordRepository _borrowRecordRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IMemoryCache _cache;

    public BorrowRecordService(
        IBorrowRecordRepository borrowRecordRepository,
        IBookRepository bookRepository,
        IMemberRepository memberRepository,
        IMemoryCache cache)
    {
        _borrowRecordRepository = borrowRecordRepository;
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
        _cache = cache;
    }

    public IEnumerable<BorrowRecordResponse> GetBorrowRecords()
    {
        return _borrowRecordRepository.GetAll().Select(MapToResponse);
    }

    public IEnumerable<BorrowRecordResponse> GetBorrowRecordsByMemberId(Guid memberId)
    {
        return _borrowRecordRepository.GetByMemberId(memberId).Select(MapToResponse);
    }

    public async Task<BorrowRecordResponse> BorrowBook(CreateBorrowRequest request)
    {
        var book = _bookRepository.GetById(request.BookId)
            ?? throw new InvalidOperationException("Book not found.");

        var member = _memberRepository.GetById(request.MemberId)
            ?? throw new InvalidOperationException("Member not found.");

        if (_borrowRecordRepository.HasActiveBorrow(request.MemberId, request.BookId))
            throw new InvalidOperationException("Member already has an active borrow for this book.");

        // Atomic conditional decrement: succeeds only if AvailableCopies > 0 at the DB level.
        // This prevents two simultaneous requests from both borrowing the last copy.
        bool reserved = await _bookRepository.TryDecrementAvailableCopiesAsync(request.BookId);
        if (!reserved)
            throw new InvalidOperationException("No copies of this book are available.");

        // Invalidate cached book data so reads reflect the updated AvailableCopies.
        _cache.Remove("books_all");
        _cache.Remove($"book_{request.BookId}");

        var record = new BorrowRecord
        {
            Id = Guid.NewGuid(),
            BookId = request.BookId,
            MemberId = request.MemberId,
            BorrowDate = DateTime.UtcNow,
            Status = "Borrowed",
            Book = book,
            Member = member
        };

        var created = _borrowRecordRepository.Add(record);
        return MapToResponse(created);
    }

    public async Task<BorrowRecordResponse> ReturnBook(Guid borrowRecordId)
    {
        var record = _borrowRecordRepository.GetById(borrowRecordId)
            ?? throw new InvalidOperationException("Borrow record not found.");

        if (record.Status != "Borrowed")
            throw new InvalidOperationException("This book has already been returned.");

        record.Status = "Returned";
        record.ReturnDate = DateTime.UtcNow;

        var updated = _borrowRecordRepository.Update(record);

        await _bookRepository.IncrementAvailableCopiesAsync(record.BookId);

        // Invalidate cached book data so reads reflect the updated AvailableCopies.
        _cache.Remove("books_all");
        _cache.Remove($"book_{record.BookId}");

        return MapToResponse(updated);
    }

    private static BorrowRecordResponse MapToResponse(BorrowRecord br) => new()
    {
        Id = br.Id,
        BookId = br.BookId,
        MemberId = br.MemberId,
        BookTitle = br.Book?.Title ?? string.Empty,
        MemberFullName = br.Member?.FullName ?? string.Empty,
        BorrowDate = br.BorrowDate,
        ReturnDate = br.ReturnDate,
        Status = br.Status
    };
}