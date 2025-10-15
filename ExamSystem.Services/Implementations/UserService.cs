using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.DTOs;
using ExamSystem.Domain.Enums;
using ExamSystem.Repository.Interfaces;
using ExamSystem.Infrastructure.Utils;
using ExamSystem.Infrastructure.Common;
using ExamSystem.Services.Interfaces;

namespace ExamSystem.Services.Implementations
{
    /// <summary>
    /// 用户服务实现
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<UserLoginResult> LoginAsync(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return new UserLoginResult
                    {
                        Success = false,
                        ErrorMessage = "用户名或密码不能为空"
                    };
                }

                var user = await _userRepository.GetByUsernameAsync(username);

                if (user == null)
                {
                    return new UserLoginResult
                    {
                        Success = false,
                        ErrorMessage = "用户名或密码错误"
                    };
                }

                if (!user.IsActive)
                {
                    return new UserLoginResult
                    {
                        Success = false,
                        ErrorMessage = "用户已被禁用"
                    };
                }

                if (!PasswordHelper.VerifyPassword(password, user.PasswordHash))
                {
                    return new UserLoginResult
                    {
                        Success = false,
                        ErrorMessage = "用户名或密码错误"
                    };
                }

                // 更新最后登录时间
                user.LastLoginAt = DateTime.Now;
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();

                return new UserLoginResult
                {
                    Success = true,
                    UserId = user.UserId,
                    Username = user.Username,
                    RealName = user.RealName,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                return new UserLoginResult
                {
                    Success = false,
                    ErrorMessage = $"登录失败: {ex.Message}"
                };
            }
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<User> CreateUserAsync(User user, string password)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("密码不能为空", nameof(password));

            // 检查用户名是否已存在
            if (await _userRepository.UsernameExistsAsync(user.Username))
                throw new InvalidOperationException("用户名已存在");

            // 加密密码
            user.PasswordHash = PasswordHelper.HashPassword(password);
            user.CreatedAt = DateTime.Now;
            user.IsActive = true;

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            await _userRepository.UpdateAsync(user);
            var result = await _userRepository.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            await _userRepository.DeleteAsync(user);
            var result = await _userRepository.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            // 验证旧密码
            if (!PasswordHelper.VerifyPassword(oldPassword, user.PasswordHash))
                return false;

            // 验证新密码强度
            if (!PasswordHelper.ValidatePasswordStrength(newPassword))
                throw new ArgumentException("新密码不符合强度要求");

            // 更新密码
            user.PasswordHash = PasswordHelper.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        public async Task<string> ResetPasswordAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("用户不存在");

            // 生成随机密码
            var newPassword = PasswordHelper.GenerateRandomPassword();
            user.PasswordHash = PasswordHelper.HashPassword(newPassword);

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return newPassword;
        }

        public bool ValidatePermission(UserRole userRole, string permission)
        {
            // 管理员拥有所有权限
            if (userRole == UserRole.Admin)
                return true;

            // 根据具体权限进行判断
            // 这里可以扩展更复杂的权限逻辑
            return false;
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            return await _userRepository.GetByRoleAsync(role);
        }

        public async Task<ImportResult> ImportUsersAsync(IEnumerable<User> users)
        {
            var result = new ImportResult();

            foreach (var user in users)
            {
                try
                {
                    // 检查用户名是否已存在
                    if (await _userRepository.UsernameExistsAsync(user.Username))
                    {
                        result.FailureCount++;
                        result.Errors.Add($"用户名 {user.Username} 已存在");
                        continue;
                    }

                    // 设置默认密码
                    user.PasswordHash = PasswordHelper.HashPassword(Constants.DefaultPassword);
                    user.CreatedAt = DateTime.Now;
                    user.IsActive = true;

                    await _userRepository.AddAsync(user);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.FailureCount++;
                    result.Errors.Add($"导入用户 {user.Username} 失败: {ex.Message}");
                }
            }

            if (result.SuccessCount > 0)
            {
                await _userRepository.SaveChangesAsync();
            }

            return result;
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _userRepository.UsernameExistsAsync(username);
        }

        /// <summary>
        /// 分页获取用户列表
        /// </summary>
        public async Task<PagedResult<User>> GetUsersAsync(int pageIndex, int pageSize)
        {
            // 获取所有用户
            var allUsers = await _userRepository.GetAllAsync();
            var userList = allUsers.ToList();
            
            // 计算分页信息
            var totalCount = userList.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            
            // 应用分页
            var items = userList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            // 返回分页结果
            return new PagedResult<User>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}
