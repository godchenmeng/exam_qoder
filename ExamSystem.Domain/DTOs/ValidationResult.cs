using System.Collections.Generic;

namespace ExamSystem.Domain.DTOs
{
    /// <summary>
    /// 验证结果DTO
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// 是否验证通过
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 错误消息列表
        /// </summary>
        public List<string> Errors { get; set; }

        public ValidationResult()
        {
            Errors = new List<string>();
        }

        /// <summary>
        /// 添加错误
        /// </summary>
        public void AddError(string error)
        {
            IsValid = false;
            Errors.Add(error);
        }
    }
}
