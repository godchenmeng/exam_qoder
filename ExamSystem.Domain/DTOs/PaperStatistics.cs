using System.Collections.Generic;

namespace ExamSystem.Domain.DTOs
{
    /// <summary>
    /// 试卷统计信息
    /// </summary>
    public class PaperStatistics
    {
        /// <summary>
        /// 试卷ID
        /// </summary>
        public int PaperId { get; set; }

        /// <summary>
        /// 试卷名称
        /// </summary>
        public string PaperName { get; set; }

        /// <summary>
        /// 考试总人数
        /// </summary>
        public int TotalExams { get; set; }

        /// <summary>
        /// 已提交人数
        /// </summary>
        public int SubmittedCount { get; set; }

        /// <summary>
        /// 已评分人数
        /// </summary>
        public int GradedCount { get; set; }

        /// <summary>
        /// 及格人数
        /// </summary>
        public int PassedCount { get; set; }

        /// <summary>
        /// 及格率
        /// </summary>
        public decimal PassRate { get; set; }

        /// <summary>
        /// 平均分
        /// </summary>
        public decimal AverageScore { get; set; }

        /// <summary>
        /// 最高分
        /// </summary>
        public decimal HighestScore { get; set; }

        /// <summary>
        /// 最低分
        /// </summary>
        public decimal LowestScore { get; set; }

        /// <summary>
        /// 分数段分布（0-60, 60-70, 70-80, 80-90, 90-100）
        /// </summary>
        public Dictionary<string, int> ScoreDistribution { get; set; }

        public PaperStatistics()
        {
            ScoreDistribution = new Dictionary<string, int>
            {
                { "0-60", 0 },
                { "60-70", 0 },
                { "70-80", 0 },
                { "80-90", 0 },
                { "90-100", 0 }
            };
        }
    }
}
