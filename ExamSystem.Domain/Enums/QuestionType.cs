namespace ExamSystem.Domain.Enums
{
    /// <summary>
    /// 题型枚举
    /// </summary>
    public enum QuestionType
    {
        /// <summary>
        /// 单选题
        /// </summary>
        SingleChoice = 1,

        /// <summary>
        /// 多选题
        /// </summary>
        MultipleChoice = 2,

        /// <summary>
        /// 判断题
        /// </summary>
        TrueFalse = 3,

        /// <summary>
        /// 填空题
        /// </summary>
        FillBlank = 4,

        /// <summary>
        /// 主观题
        /// </summary>
        Subjective = 5,

        ShortAnswer = 6,

        Essay = 7
    }
}
