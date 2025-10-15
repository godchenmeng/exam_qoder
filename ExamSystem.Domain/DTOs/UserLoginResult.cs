using ExamSystem.Domain.Enums;

namespace ExamSystem.Domain.DTOs
{
    /// <summary>
    /// 用户登录结果DTO
    /// </summary>
    public class UserLoginResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public UserRole Role { get; set; }
    }
}
