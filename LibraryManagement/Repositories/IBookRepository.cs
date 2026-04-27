using LibraryManagement.Api.Models;

namespace LibraryManagement.Api.Repositories;

public interface IBookRepository
{
    IEnumerable<Book> GetAll();
    Book? GetById(Guid id);
    Book Add(Book book);
    Book Update(Book book);
    void Delete(Book book);
    bool ExistsByIsbn(string isbn);
    Task<bool> TryDecrementAvailableCopiesAsync(Guid bookId);
    Task IncrementAvailableCopiesAsync(Guid bookId);
}