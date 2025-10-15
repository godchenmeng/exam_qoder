using System.Collections.Generic;
using System.Threading.Tasks;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.DTOs;
using ExamSystem.Domain.Enums;

namespace ExamSystem.Services.Interfaces
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        Task<UserLoginResult> LoginAsync(string username, string password);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        Task<User> GetUserByIdAsync(int userId);

        /// <summary>
        /// 创建用户
        /// </summary>
        Task<User> CreateUserAsync(User user, string password);

        /// <summary>
        /// 更新用户信息
        /// </summary>
        Task<bool> UpdateUserAsync(User user);

        /// <summary>
        /// 删除用户
        /// </summary>
        Task<bool> DeleteUserAsync(int userId);

        /// <summary>
        /// 修改密码
        /// </summary>
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);

        /// <summary>
        /// 重置密码
        /// </summary>
        Task<string> ResetPasswordAsync(int userId);

        /// <summary>
        /// 验证权限
        /// </summary>
        bool ValidatePermission(UserRole userRole, string permission);

        /// <summary>
        /// 根据角色获取用户列表
        /// </summary>
        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);

        /// <summary>
        /// 批量导入用户
        /// </summary>
        Task<ImportResult> ImportUsersAsync(IEnumerable<User> users);

        /// <summary>
        /// 检查用户名是否存在
        /// </summary>
        Task<bool> UsernameExistsAsync(string username);
        
        // 新增的方法
        /// <summary>
        /// 分页获取用户列表
        /// </summary>
        Task<PagedResult<User>> GetUsersAsync(int pageIndex, int pageSize);
    }

    /// <summary>
    /// 导入结果
    /// </summary>
    public class ImportResult
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}