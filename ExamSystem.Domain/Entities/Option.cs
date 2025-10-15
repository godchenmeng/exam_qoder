namespace ExamSystem.Domain.Entities
{
    /// <summary>
    /// 选项实体
    /// </summary>
    public class Option
    {
        /// <summary>
        /// 选项唯一标识
        /// </summary>
        public int OptionId { get; set; }

        /// <summary>
        /// 所属题目ID
        /// </summary>
        public int QuestionId { get; set; }

        /// <summary>
        /// 所属题目导航属性
        /// </summary>
        public Question Question { get; set; }

        /// <summary>
        /// 选项内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 是否正确答案
        /// </summary>
        public bool IsCorrect { get; set; }

        /// <summary>
        /// 排序索引
        /// </summary>
        public int OrderIndex { get; set; }
    }
}
