namespace ExamSystem.Domain.DTOs
{
    /// <summary>
    /// 题目分析结果
    /// </summary>
    public class QuestionAnalysis
    {
        /// <summary>
        /// 题目ID
        /// </summary>
        public int QuestionId { get; set; }

        /// <summary>
        /// 题目内容
        /// </summary>
        public string QuestionContent { get; set; }

        /// <summary>
        /// 答题总人数
        /// </summary>
        public int TotalAnswers { get; set; }

        /// <summary>
        /// 答对人数
        /// </summary>
        public int CorrectAnswers { get; set; }

        /// <summary>
        /// 正确率
        /// </summary>
        public decimal CorrectRate { get; set; }

        /// <summary>
        /// 平均得分
        /// </summary>
        public decimal AverageScore { get; set; }

        /// <summary>
        /// 题目满分
        /// </summary>
        public decimal FullScore { get; set; }

        /// <summary>
        /// 得分率
        /// </summary>
        public decimal ScoreRate { get; set; }

        /// <summary>
        /// 区分度（高分组正确率 - 低分组正确率）
        /// </summary>
        public decimal Discrimination { get; set; }
    }
}
