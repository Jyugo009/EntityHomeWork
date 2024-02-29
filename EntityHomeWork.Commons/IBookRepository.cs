using EntityHomeWork.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityHomeWork.Commons
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooks();

        Task<Book?> GetBookById(int id);

        Task AddBook(Book book);

        Task UpdateBook(Book book);

        Task DeleteBook(int id);

        Task BorrowBook(Book book, Reader reader);

        Task<IEnumerable<Book>> GetBorrowedBooks(Reader reader);
    }
}
