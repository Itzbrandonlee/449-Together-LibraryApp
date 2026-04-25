using LibraryManagement.Api.Dtos;
using LibraryManagement.Api.Exceptions;
using LibraryManagement.Api.Models;
using LibraryManagement.Api.Repositories;

namespace LibraryManagement.Api.Services;

public class BorrowRecordService : IBorrowRecordService
{
    private readonly IBorrowRecordRepository _borrowRecordRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;

    public BorrowRecordService(
        IBorrowRecordRepository borrowRecordRepository,
        IBookRepository bookRepository,
        IMemberRepository memberRepository)
    {
        _borrowRecordRepository = borrowRecordRepository;
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
    }

    public async Task<IEnumerable<BorrowRecordResponse>> GetBorrowRecordsAsync()
    {
        var records = await _borrowRecordRepository.GetAllAsync();
        return records.Select(MapToResponse);
    }

    public async Task<IEnumerable<BorrowRecordResponse>> GetBorrowRecordsByMemberIdAsync(Guid memberId)
    {
        var records = await _borrowRecordRepository.GetByMemberIdAsync(memberId);
        return records.Select(MapToResponse);
    }

    public async Task<BorrowRecordResponse> BorrowBookAsync(CreateBorrowRequest request)
    {
        var book = await _bookRepository.GetByIdAsync(request.BookId)
            ?? throw new InvalidOperationException("Book not found.");

        var member = await _memberRepository.GetByIdAsync(request.MemberId)
            ?? throw new InvalidOperationException("Member not found.");

        if (book.AvailableCopies <= 0)
            throw new ConflictException("No copies of this book are currently available.");

        if (await _borrowRecordRepository.HasActiveBorrowAsync(request.MemberId, request.BookId))
            throw new ConflictException("Member already has an active borrow for this book.");

        book.AvailableCopies--;
        await _bookRepository.UpdateAsync(book);

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

        var created = await _borrowRecordRepository.AddAsync(record);
        return MapToResponse(created);
    }

    public async Task<BorrowRecordResponse> ReturnBookAsync(Guid borrowRecordId)
    {
        var record = await _borrowRecordRepository.GetByIdAsync(borrowRecordId)
            ?? throw new InvalidOperationException("Borrow record not found.");

        if (record.Status != "Borrowed")
            throw new InvalidOperationException("This book has already been returned.");

        record.Status = "Returned";
        record.ReturnDate = DateTime.UtcNow;
        record.Book!.AvailableCopies++;

        await _bookRepository.UpdateAsync(record.Book);
        var updated = await _borrowRecordRepository.UpdateAsync(record);
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
