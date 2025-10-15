namespace ExamSystem.Domain.Enums
{
    /// <summary>
    /// 试卷类型枚举
    /// </summary>
    public enum PaperType
    {
        /// <summary>
        /// 固定试卷 - 所有考生题目相同
        /// </summary>
        Fixed = 1,

        /// <summary>
        /// 随机试卷 - 按规则随机抽题
        /// </summary>
        Random = 2,

        /// <summary>
        /// 混合试卷 - 部分固定+部分随机
        /// </summary>
        Mixed = 3
    }
}
