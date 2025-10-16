using System.Collections.Generic;
using ExamSystem.Domain.Enums;

namespace ExamSystem.Domain.DTOs
{
    /// <summary>
    /// 随机组卷配置
    /// </summary>
    public class RandomPaperConfig
    {
        /// <summary>
        /// 题库ID
        /// </summary>
        public int BankId { get; set; }

        /// <summary>
        /// 抽题规则列表
        /// </summary>
        public List<QuestionSelectionRule> Rules { get; set; }

        public RandomPaperConfig()
        {
            Rules = new List<QuestionSelectionRule>();
        }
    }

    /// <summary>
    /// 题目选择规则
    /// </summary>
    public class QuestionSelectionRule
    {
        /// <summary>
        /// 题型
        /// </summary>
        public QuestionType QuestionType { get; set; }

        /// <summary>
        /// 难度（可选）
        /// </summary>
        public Difficulty? Difficulty { get; set; }

        /// <summary>
        /// 抽取数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 每题分值
        /// </summary>
        public double ScorePerQuestion { get; set; }
    }
}
