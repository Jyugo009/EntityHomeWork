using EntityHomeWork.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityHomeWork.Commons
{
    public interface IUserRepository
    {
        Task<Librarian?> GetLibrarianByLogin(string login);

        Task<Reader?> GetReaderByLogin(string login);

        Task<IEnumerable<Reader>> GetAllReaders();

        Task<Librarian> CreateLibrarian(Librarian librarian);

        Task<Reader> CreateReader(Reader reader);

        Task<Librarian?> UpdateLibrarian(Librarian librarian);

        Task<Reader?> UpdateReader(Reader reader);

        Task<Librarian?> DeleteLibrarian(string login);

        Task<Reader?> DeleteReader(string login);
    }
}
