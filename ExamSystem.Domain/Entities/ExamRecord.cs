using System;
using ExamSystem.Domain.Enums;

namespace ExamSystem.Domain.Entities
{
    /// <summary>
    /// 考试记录实体
    /// </summary>
    public class ExamRecord
    {
        /// <summary>
        /// 记录唯一标识
        /// </summary>
        public int RecordId { get; set; }

        /// <summary>
        /// 考生ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 考生导航属性
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// 试卷ID
        /// </summary>
        public int PaperId { get; set; }

        /// <summary>
        /// 试卷导航属性
        /// </summary>
        public ExamPaper ExamPaper { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime? SubmitTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ExamStatus Status { get; set; }

        /// <summary>
        /// 总得分
        /// </summary>
        public double TotalScore { get; set; }

        /// <summary>
        /// 客观题得分
        /// </summary>
        public double ObjectiveScore { get; set; }

        /// <summary>
        /// 主观题得分
        /// </summary>
        public double SubjectiveScore { get; set; }

        /// <summary>
        /// 是否通过
        /// </summary>
        public bool? IsPassed { get; set; }

        /// <summary>
        /// 异常行为记录(JSON)
        /// </summary>
        public string AbnormalBehaviors { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
