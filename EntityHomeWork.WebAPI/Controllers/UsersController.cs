using EntityHomeWork.Commons;
using EntityHomeWork.DBModels;
using EntityHomeWork.WebAPI.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace EntityHomeWork.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IHashService _hashService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register/librarian")]
        public async Task<ActionResult> RegisterLibrarian(RegisterLibrarianDTO registerDto)
        {
            var newUser = new Librarian { Login = registerDto.Login, Email = registerDto.Email };

            var (user, token) = await _userService.Register(newUser, registerDto.Password);

            return Ok(new { user, token });
        }

        [HttpPost("register/reader")]
        public async Task<ActionResult> RegisterReader(RegisterReaderDTO registerDto)
        {
            var newUser = new Reader
            {
                Login = registerDto.Login,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                DocumentTypeId = registerDto.DocumentTypeId.Value,
                DocumentNumber = registerDto.DocumentNumber
            };

            var (user, token) = await _userService.Register(newUser, registerDto.Password);

            return Ok(new { user, token });
        }

        [HttpPut("librarians/update/{login}")]
        [Authorize]
        public async Task<IActionResult> UpdateLibrarian(string login, [FromBody] UpdateLibrarianDTO librarianData)
        {
            var librarian = await _userRepository.GetLibrarianByLogin(login);

            if (librarian != null)
            {
                var hashResult = _hashService.GetHash(librarianData.Password);
                librarian.PasswordHash = hashResult.hash;

                librarian.PasswordSalt = hashResult.salt;

                librarian.Email = librarianData.Email;

                await _userRepository.UpdateLibrarian(librarian);

                return NoContent();
            }

            return NotFound();
        }

        [HttpPut("readers/update/{login}")]
        [Authorize]
        public async Task<IActionResult> UpdateReader(string login, [FromBody] UpdateReaderDTO readerData)
        {
            var reader = await _userRepository.GetReaderByLogin(login);

            if (reader != null)
            {
                var hashResult = _hashService.GetHash(readerData.Password);
                reader.PasswordHash = hashResult.hash;
              

                reader.Email = readerData.Email;
                reader.FirstName = readerData.FirstName;
                reader.LastName = readerData.LastName;

                await _userRepository.UpdateReader(reader);

                return NoContent();
            }

            return NotFound();
        }

        [HttpDelete("readers/{login}")]
        public async Task<IActionResult> DeleteReader(string login)
        {
            var reader = await _userRepository.DeleteReader(login);

            if (reader == null)
            {
                return NotFound();
            }

            return Ok(reader);
        }

        [HttpGet("readers")]
        public async Task<ActionResult<IEnumerable<Reader>>> GetReaders()
        {
            var readers = await _userRepository.GetAllReaders();

            return Ok(readers);
        }

    }


}


