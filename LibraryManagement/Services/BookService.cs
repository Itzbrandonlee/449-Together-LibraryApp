using LibraryManagement.Api.Dtos;
using LibraryManagement.Api.Models;
using LibraryManagement.Api.Repositories;

namespace LibraryManagement.Api.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<IEnumerable<BookResponse>> GetBooksAsync()
    {
        var books = await _bookRepository.GetAllAsync();
        return books.Select(b => new BookResponse
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            ISBN = b.ISBN,
            TotalCopies = b.TotalCopies,
            AvailableCopies = b.AvailableCopies
        });
    }

    public async Task<BookResponse?> GetBookByIdAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book is null)
            return null;

        return new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            ISBN = book.ISBN,
            TotalCopies = book.TotalCopies,
            AvailableCopies = book.AvailableCopies
        };
    }

    public async Task<BookResponse> CreateBookAsync(CreateBookRequest request)
    {
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Author = request.Author,
            ISBN = request.ISBN,
            TotalCopies = request.TotalCopies,
            AvailableCopies = request.TotalCopies
        };

        var created = await _bookRepository.AddAsync(book);
        return new BookResponse
        {
            Id = created.Id,
            Title = created.Title,
            Author = created.Author,
            ISBN = created.ISBN,
            TotalCopies = created.TotalCopies,
            AvailableCopies = created.AvailableCopies
        };
    }

    public async Task<BookResponse?> UpdateBookAsync(Guid id, UpdateBookRequest request)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book is null)
            return null;

        if (request.AvailableCopies > request.TotalCopies)
            throw new InvalidOperationException("AvailableCopies cannot exceed TotalCopies.");

        book.Title = request.Title;
        book.Author = request.Author;
        book.ISBN = request.ISBN;
        book.TotalCopies = request.TotalCopies;
        book.AvailableCopies = request.AvailableCopies;

        var updated = await _bookRepository.UpdateAsync(book);
        return new BookResponse
        {
            Id = updated.Id,
            Title = updated.Title,
            Author = updated.Author,
            ISBN = updated.ISBN,
            TotalCopies = updated.TotalCopies,
            AvailableCopies = updated.AvailableCopies
        };
    }

    public async Task DeleteBookAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException("Book not found.");

        await _bookRepository.DeleteAsync(book);
    }
}