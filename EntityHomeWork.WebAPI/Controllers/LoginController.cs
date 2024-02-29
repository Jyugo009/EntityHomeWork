using EntityHomeWork.Commons;
using EntityHomeWork.DBModels;
using EntityHomeWork.WebAPI.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static EntityHomeWork.WebAPI.Dtos.UsersDTO;

namespace EntityHomeWork.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var (user, token) = await _userService.Login(loginDto.Login, loginDto.Password);

            if (user == null)
                return Unauthorized("Invalid username or password");

            var userDto = MapUserToDto(user);

            return Ok(new { user = userDto, token });
        }

        private object MapUserToDto(object user)
        {
            if (user is Librarian librarian)
            {
                return new Reader
                {
                    Login = librarian.Login,
                    Email = librarian.Email,
                };
            }

            if (user is Reader reader)
            {
                return new ReaderDto
                {
                    Login = reader.Login,
                    Email = reader.Email,
                    FirstName = reader.FirstName,
                    LastName = reader.LastName,
                };
            }

            throw new ArgumentException("Invalid user type");
        }

    }
}

    
