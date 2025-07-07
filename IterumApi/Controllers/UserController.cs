using IterumApi.DTOs;
using IterumApi.Models;
using IterumApi.Repositories;
using IterumApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IterumApi.Controllers
{
    [Route("iterum/api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly IConfiguration _configuration;

        public UserController(IUserRepo userRepo, IConfiguration configuration)
        {
            _userRepo = userRepo;
            _configuration = configuration;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterForm registerData)
        {
            Console.WriteLine(Request.Body);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                User? user = await _userRepo.RegisterAsync(registerData);

                if (user == null)
                {
                    return Conflict("That user already exists");
                }

                var secureKey = _configuration["JWT:SecureKey"];

                var serializedToken =
                    JwtTokenProvider.CreateToken(
                        secureKey,
                        120,
                        user.Username,
                        user.Id);

                return Ok(new LoginResponse(user.Username, serializedToken));
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.StartsWith("Violation of UNIQUE KEY constraint "))
                {
                    return BadRequest("That user already exists");
                }
                return StatusCode(500);
            }

        }

        [HttpPost("[action]")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginForm loginData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                User? user = await _userRepo.LoginAsync(loginData);
                if (user == null)
                {
                    return Unauthorized("Bad username or password");
                }

                var secureKey = _configuration["JWT:SecureKey"];

                var serializedToken =
                    JwtTokenProvider.CreateToken(
                        secureKey,
                        120,
                        user.Username,
                        user.Id);

                return Ok(new LoginResponse(user.Username, serializedToken));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
