using System.Collections.Generic;

namespace ExamSystem.Domain.DTOs
{
    /// <summary>
    /// 混合组卷配置
    /// </summary>
    public class MixedPaperConfig
    {
        /// <summary>
        /// 固定题目ID列表
        /// </summary>
        public List<int> FixedQuestionIds { get; set; }

        /// <summary>
        /// 固定题目分值字典（题目ID -> 分值）
        /// </summary>
        public Dictionary<int, decimal> FixedQuestionScores { get; set; }

        /// <summary>
        /// 随机组卷配置
        /// </summary>
        public RandomPaperConfig RandomConfig { get; set; }

        public MixedPaperConfig()
        {
            FixedQuestionIds = new List<int>();
            FixedQuestionScores = new Dictionary<int, decimal>();
        }
    }
}
