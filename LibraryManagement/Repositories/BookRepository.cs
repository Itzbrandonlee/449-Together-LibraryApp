using LibraryManagement.Api.Data;
using LibraryManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Api.Repositories;

public class BookRepository : IBookRepository
{
    private readonly ApplicationDbContext _context;

    public BookRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Book> GetAll()
    {
        return _context.Books.ToList();
    }

    public Book? GetById(Guid id)
    {
        return _context.Books.FirstOrDefault(b => b.Id == id);
    }

    public Book Add(Book book)
    {
        _context.Books.Add(book);
        _context.SaveChanges();
        return book;
    }

    public Book Update(Book book)
    {
        _context.Books.Update(book);
        _context.SaveChanges();
        return book;
    }

    public void Delete(Book book)
    {
        _context.Books.Remove(book);
        _context.SaveChanges();
    }

    public bool ExistsByIsbn(string isbn)
    {
        return _context.Books.Any(b => b.ISBN == isbn);
    }

    public async Task<bool> TryDecrementAvailableCopiesAsync(Guid bookId)
    {
        var rowsAffected = await _context.Books
            .Where(b => b.Id == bookId && b.AvailableCopies > 0)
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.AvailableCopies, b => b.AvailableCopies - 1));
        return rowsAffected > 0;
    }

    public async Task IncrementAvailableCopiesAsync(Guid bookId)
    {
        await _context.Books
            .Where(b => b.Id == bookId)
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.AvailableCopies, b => b.AvailableCopies + 1));
    }
}