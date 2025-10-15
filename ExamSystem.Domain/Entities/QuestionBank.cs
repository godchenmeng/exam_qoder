using System;

namespace ExamSystem.Domain.Entities
{
    /// <summary>
    /// 题库实体
    /// </summary>
    public class QuestionBank
    {
        /// <summary>
        /// 题库唯一标识
        /// </summary>
        public int BankId { get; set; }

        /// <summary>
        /// 题库名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 题库描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        public int CreatorId { get; set; }

        /// <summary>
        /// 创建者导航属性
        /// </summary>
        public User Creator { get; set; }

        /// <summary>
        /// 是否公开
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
