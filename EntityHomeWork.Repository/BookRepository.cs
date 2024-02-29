using EntityHomeWork.Commons;
using EntityHomeWork.DBModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityHomeWork.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryContext _context;

        public BookRepository(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllBooks()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Book?> GetBookById(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task AddBook(Book book)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));
            await _context.Books.AddAsync(book);
            await SaveChanges();
        }

        public async Task UpdateBook(Book book)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));
            _context.Entry(book).State = EntityState.Modified;
            await SaveChanges();
        }

        public async Task DeleteBook(int id)
        {
            var book = await GetBookById(id);

            if (book != null)
            {
                _context.Books.Remove(book);
                await SaveChanges();
            }
        }

        public async Task BorrowBook(Book book, Reader reader)
        {
            var bookOnHand = new BookOnHand
            {
                TakenBy = reader.Login,
                CheckoutDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(30),
                BookId = book.BookId
            };

            await _context.BookOnHands.AddAsync(bookOnHand);
            await SaveChanges();
        }

        public async Task<IEnumerable<Book>> GetBorrowedBooks(Reader reader)
        {
            return await _context.BookOnHands.Where(b => b.TakenBy == reader.Login).Select(b => b.Book).ToListAsync();
        }

        private async Task SaveChanges()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception("Could not save changes", ex);
            }
        }
    }
}
