namespace ExamSystem.Domain.DTOs
{
    /// <summary>
    /// 评分项
    /// </summary>
    public class GradeItem
    {
        /// <summary>
        /// 答题记录ID
        /// </summary>
        public int AnswerRecordId { get; set; }

        /// <summary>
        /// 分数
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// 评语
        /// </summary>
        public string Comment { get; set; }
    }
}
