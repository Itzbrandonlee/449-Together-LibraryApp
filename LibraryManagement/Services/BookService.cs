using LibraryManagement.Api.Dtos;
using LibraryManagement.Api.Models;
using LibraryManagement.Api.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace LibraryManagement.Api.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMemoryCache _cache;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(2);

    public BookService(IBookRepository bookRepository, IMemoryCache cache)
    {
        _bookRepository = bookRepository;
        _cache = cache;
    }

    public IEnumerable<BookResponse> GetBooks()
    {
        if (_cache.TryGetValue("books_all", out IEnumerable<BookResponse>? cached) && cached is not null)
            return cached;

        var books = _bookRepository.GetAll()
            .Select(b => new BookResponse
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                ISBN = b.ISBN,
                TotalCopies = b.TotalCopies,
                AvailableCopies = b.AvailableCopies
            })
            .ToList();

        _cache.Set("books_all", books, CacheDuration);
        return books;
    }

    public BookResponse? GetBookById(Guid id)
    {
        string key = $"book_{id}";
        if (_cache.TryGetValue(key, out BookResponse? cached) && cached is not null)
            return cached;

        var book = _bookRepository.GetById(id);
        if (book is null)
            return null;

        var response = new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            ISBN = book.ISBN,
            TotalCopies = book.TotalCopies,
            AvailableCopies = book.AvailableCopies
        };

        _cache.Set(key, response, CacheDuration);
        return response;
    }

    public BookResponse CreateBook(CreateBookRequest request)
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

        var created = _bookRepository.Add(book);

        _cache.Remove("books_all");

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

    public BookResponse UpdateBook(Guid id, UpdateBookRequest request)
    {
        var book = _bookRepository.GetById(id)
            ?? throw new InvalidOperationException("Book not found.");

        if (request.AvailableCopies > request.TotalCopies)
            throw new InvalidOperationException("AvailableCopies cannot exceed TotalCopies.");

        book.Title = request.Title;
        book.Author = request.Author;
        book.ISBN = request.ISBN;
        book.TotalCopies = request.TotalCopies;
        book.AvailableCopies = request.AvailableCopies;

        var updated = _bookRepository.Update(book);

        _cache.Remove("books_all");
        _cache.Remove($"book_{id}");

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

    public void DeleteBook(Guid id)
    {
        var book = _bookRepository.GetById(id)
            ?? throw new InvalidOperationException("Book not found.");

        _bookRepository.Delete(book);

        _cache.Remove("books_all");
        _cache.Remove($"book_{id}");
    }
}