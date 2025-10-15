namespace ExamSystem.Domain.Entities
{
    /// <summary>
    /// 试卷题目关联实体
    /// </summary>
    public class PaperQuestion
    {
        /// <summary>
        /// 关联记录ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 试卷ID
        /// </summary>
        public int PaperId { get; set; }

        /// <summary>
        /// 试卷导航属性
        /// </summary>
        public ExamPaper ExamPaper { get; set; }

        /// <summary>
        /// 题目ID
        /// </summary>
        public int QuestionId { get; set; }

        /// <summary>
        /// 题目导航属性
        /// </summary>
        public Question Question { get; set; }

        /// <summary>
        /// 题目顺序
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// 该题分值
        /// </summary>
        public decimal Score { get; set; }
    }
}
