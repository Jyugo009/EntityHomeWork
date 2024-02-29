using EntityHomeWork.Commons;
using EntityHomeWork.DBModels;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace EntityHomeWork.Repository
{
    internal class UserRepository : IUserRepository
    {
        private readonly LibraryContext _context;

        public UserRepository(LibraryContext context)
        {
            _context = context;
        }

        public Task<Librarian?> GetLibrarianByLogin(string login)
        {
            return _context.Librarians.FirstOrDefaultAsync(u => u.Login == login);
        }

        public Task<Reader?> GetReaderByLogin(string login)
        {
            return _context.Readers.FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task<IEnumerable<Reader>> GetAllReaders()
        {
            return await _context.Readers.ToListAsync();
        }

        public async Task<Librarian> CreateLibrarian(Librarian librarian)
        {
            await _context.Librarians.AddAsync(librarian);
            await _context.SaveChangesAsync();
            return librarian;
        }

        public async Task<Reader> CreateReader(Reader reader)
        {
            await _context.Readers.AddAsync(reader);
            await _context.SaveChangesAsync();
            return reader;
        }

        public async Task<Librarian?> UpdateLibrarian(Librarian librarian)
        {
            _context.Attach(librarian).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();                
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception($"Could not update Librarian with login {librarian.Login}");
            }

            return librarian;
        }

        public async Task<Reader> UpdateReader(Reader reader)
        {
            _context.Attach(reader).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return reader;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception($"Could not update Reader with login {reader.Login}");
            }
        }

        public async Task<Librarian?> DeleteLibrarian(string login)
        {
            var librarian = await GetLibrarianByLogin(login);

            if (librarian != null)
            {
                _context.Librarians.Remove(librarian);
                await _context.SaveChangesAsync();
            }

            return librarian;
        }

        public async Task<Reader?> DeleteReader(string login)
        {
            var reader = await GetReaderByLogin(login);

            if (reader != null)
            {
                _context.Readers.Remove(reader);
                await _context.SaveChangesAsync();
            }

            return reader;
        }
        
    }
}
