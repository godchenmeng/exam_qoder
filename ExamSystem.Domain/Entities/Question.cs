using System;
using ExamSystem.Domain.Enums;

namespace ExamSystem.Domain.Entities
{
    /// <summary>
    /// 题目实体
    /// </summary>
    public class Question
    {
        /// <summary>
        /// 题目唯一标识
        /// </summary>
        public int QuestionId { get; set; }

        /// <summary>
        /// 所属题库ID
        /// </summary>
        public int BankId { get; set; }

        /// <summary>
        /// 所属题库导航属性
        /// </summary>
        public QuestionBank QuestionBank { get; set; }

        /// <summary>
        /// 题型
        /// </summary>
        public QuestionType QuestionType { get; set; }

        /// <summary>
        /// 题目内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 正确答案
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// 题目解析
        /// </summary>
        public string Analysis { get; set; }

        /// <summary>
        /// 默认分值
        /// </summary>
        public double DefaultScore { get; set; }

        /// <summary>
        /// 难度
        /// </summary>
        public Difficulty Difficulty { get; set; }

        /// <summary>
        /// 标签(逗号分隔)
        /// </summary>
        public string Tags { get; set; }

        public string Explanation { get; set; }

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
