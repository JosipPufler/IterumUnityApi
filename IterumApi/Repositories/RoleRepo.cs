using IterumApi.DTOs;
using IterumApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IterumApi.Repositories
{
    public class RoleRepo
    {
        private readonly IterumDbContext _context;

        public RoleRepo(IterumDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(long id)
        {
            return await _context.Roles.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _context.Roles.FirstOrDefaultAsync(x => x.Name == name);
        }
    }
}
