using EntityHomeWork.Commons;
using EntityHomeWork.DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace EntityHomeWork.WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var books = await _bookRepository.GetAllBooks();
            return Ok(books);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _bookRepository.GetBookById(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            await _bookRepository.AddBook(book);

            return CreatedAtAction(nameof(GetBook), new { id = book.BookId }, book);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutTodoItem(int id, Book updatedBook)
        {
            if (id != updatedBook.BookId) return BadRequest();

            try
            {
                await _bookRepository.UpdateBook(updatedBook);
            }
            catch (Exception)
            {
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            await _bookRepository.DeleteBook(id);

            return NoContent();
        }

        [HttpPost("{bookId}/borrow/{readerLogin}")]
        [Authorize]
        public async Task<IActionResult> BorrowBook(int bookId, string readerLogin)
        {
            var book = await _bookRepository.GetBookById(bookId);
            if (book == null) return NotFound("Book not found");

            var reader = await _userRepository.GetReaderByLogin(readerLogin);
            if (reader == null) return NotFound("Reader not found");

            await _bookRepository.BorrowBook(book, reader);

            return Ok(new { message = $"Book '{book.Title}' has been borrowed by {reader.FirstName} {reader.LastName}." });
        }

        [HttpGet("readers/{login}/borrowedBooks")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBorrowedBooks(string login)
        {
            var reader = await _userRepository.GetReaderByLogin(login);
            if (reader == null) return NotFound("Reader not found");

            var borrowedBooks = await _bookRepository.GetBorrowedBooks(reader);

            return Ok(borrowedBooks);
        }

    }
}
