using IterumApi.DTOs;
using IterumApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IterumApi.Repositories
{
    public interface IUserRepo
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(long id);
        Task<User?> GetByUserNameAsync(string username);
        Task<User?> LoginAsync(LoginForm loginForm);
        Task<User?> RegisterAsync(RegisterForm registerForm);
    }

    public class UserRepo : IUserRepo
    {
        private readonly IterumDbContext _context;

        public UserRepo(IterumDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(long id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User?> GetByUserNameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<User?> LoginAsync(LoginForm loginForm)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == loginForm.Username);
            if (user == null)
                return null;

            return BCrypt.Net.BCrypt.Verify(loginForm.Password, user.Password) ? user : null;
        }

        public async Task<User?> RegisterAsync(RegisterForm registerForm)
        {
            try
            {
                var user = new User
                {
                    Username = registerForm.Username,
                    Password = BCrypt.Net.BCrypt.HashPassword(registerForm.Password)
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate") == true)
            {
                Console.WriteLine("Username must be unique");
                return null;
            }
        }
    }
}
