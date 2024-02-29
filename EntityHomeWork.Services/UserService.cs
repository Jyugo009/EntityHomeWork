using EntityHomeWork.Commons;
using EntityHomeWork.DBModels;

namespace EntityHomeWork.Services
{
    internal class UserService(IUserRepository userRepository, ITokenService tokenService, IHashService hashService) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IHashService _hashService = hashService;

        public async Task<(object? user, string token)> Login(string login, string password)
        {
            var librarian = await _userRepository.GetLibrarianByLogin(login);

            if (librarian != null)
            {
                bool isValidPassword = _hashService.VerifyPassword(password, librarian.PasswordSalt, librarian.PasswordHash);

                if (isValidPassword)
                {
                    return (librarian, token: _tokenService.GetToken(librarian));
                }
            }

            var reader = await _userRepository.GetReaderByLogin(login);

            if (reader != null)
            {
                bool isValidPassword = _hashService.VerifyPassword(password, reader.PasswordSalt, reader.PasswordHash);

                if (isValidPassword)
                {
                    var token = _tokenService.GetToken(reader);
                    return (reader, token);
                }
            }

            throw new UnauthorizedAccessException();
        }

        public async Task<(object user, string token)> Register(object userParam, string password)
        {

            if (userParam is Librarian librarian)
            {
                var hash = _hashService.GetHash(password);

                librarian.PasswordHash = hash.hash;
                librarian.PasswordSalt = hash.salt;

                await _userRepository.CreateLibrarian(librarian);

                var token = _tokenService.GetToken(librarian);

                return (librarian, token);
            }
            else if (userParam is Reader reader)
            {

                var hash = _hashService.GetHash(password);

                reader.PasswordHash = hash.hash;
                reader.PasswordSalt = hash.salt;

                await _userRepository.CreateReader(reader);

                var token = _tokenService.GetToken(reader);

                return (reader, token);

            }

            throw new Exception("User should be either a Reader or a Librarian");
        }
    }
}
