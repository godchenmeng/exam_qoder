namespace ExamSystem.Domain.Enums
{
    /// <summary>
    /// 考试状态枚举
    /// </summary>
    public enum ExamStatus
    {
        /// <summary>
        /// 考试进行中
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// 已提交,待评分
        /// </summary>
        Submitted = 2,

        /// <summary>
        /// 已评分
        /// </summary>
        Graded = 3,

        /// <summary>
        /// 超时自动提交
        /// </summary>
        Timeout = 4
    }
}
