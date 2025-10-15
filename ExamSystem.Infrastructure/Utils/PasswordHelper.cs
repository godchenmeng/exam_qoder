using System;
using System.Security.Cryptography;
using System.Text;

namespace ExamSystem.Infrastructure.Utils
{
    /// <summary>
    /// 密码加密工具类
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>
        /// 生成密码哈希值(使用SHA256)
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <returns>哈希值</returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// 验证密码
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <param name="passwordHash">存储的哈希值</param>
        /// <returns>是否匹配</returns>
        public static bool VerifyPassword(string password, string passwordHash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordHash))
                return false;

            var hash = HashPassword(password);
            return hash == passwordHash;
        }

        /// <summary>
        /// 生成随机密码
        /// </summary>
        /// <param name="length">密码长度</param>
        /// <returns>随机密码</returns>
        public static string GenerateRandomPassword(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%";
            var random = new Random();
            var password = new char[length];

            for (int i = 0; i < length; i++)
            {
                password[i] = chars[random.Next(chars.Length)];
            }

            return new string(password);
        }

        /// <summary>
        /// 验证密码强度
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns>是否符合强度要求</returns>
        public static bool ValidatePasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 6)
                return false;

            // 可以添加更多复杂度验证
            return true;
        }
    }
}
