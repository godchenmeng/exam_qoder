using System.Collections.Generic;

namespace ExamSystem.Domain.DTOs
{
    /// <summary>
    /// 题库统计信息DTO
    /// </summary>
    public class BankStatistics
    {
        /// <summary>
        /// 题库ID
        /// </summary>
        public int BankId { get; set; }

        /// <summary>
        /// 题库名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 题目总数
        /// </summary>
        public int TotalQuestions { get; set; }

        /// <summary>
        /// 各题型数量分布
        /// </summary>
        public Dictionary<string, int> QuestionTypeDistribution { get; set; }

        /// <summary>
        /// 各难度数量分布
        /// </summary>
        public Dictionary<string, int> DifficultyDistribution { get; set; }

        public BankStatistics()
        {
            QuestionTypeDistribution = new Dictionary<string, int>();
            DifficultyDistribution = new Dictionary<string, int>();
        }
    }
}
