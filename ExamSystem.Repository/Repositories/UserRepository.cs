using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Enums;
using ExamSystem.Repository.Context;
using ExamSystem.Repository.Interfaces;

namespace ExamSystem.Repository.Repositories
{
    /// <summary>
    /// 用户仓储实现
    /// </summary>
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ExamSystemDbContext context) : base(context)
        {
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role)
        {
            return await _dbSet
                .Where(u => u.Role == role)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _dbSet
                .Where(u => u.IsActive)
                .ToListAsync();
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _dbSet
                .AnyAsync(u => u.Username == username);
        }
    }
}
