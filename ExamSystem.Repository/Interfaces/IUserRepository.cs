using System.Collections.Generic;
using System.Threading.Tasks;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Enums;

namespace ExamSystem.Repository.Interfaces
{
    /// <summary>
    /// 用户仓储接口
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        Task<User> GetByUsernameAsync(string username);

        /// <summary>
        /// 根据角色获取用户列表
        /// </summary>
        Task<IEnumerable<User>> GetByRoleAsync(UserRole role);

        /// <summary>
        /// 获取激活的用户列表
        /// </summary>
        Task<IEnumerable<User>> GetActiveUsersAsync();

        /// <summary>
        /// 检查用户名是否存在
        /// </summary>
        Task<bool> UsernameExistsAsync(string username);
    }
}
