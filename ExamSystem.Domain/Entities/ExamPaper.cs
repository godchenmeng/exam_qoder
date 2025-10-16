using System;
using ExamSystem.Domain.Enums;

namespace ExamSystem.Domain.Entities
{
    /// <summary>
    /// 试卷实体
    /// </summary>
    public class ExamPaper
    {
        /// <summary>
        /// 试卷唯一标识
        /// </summary>
        public int PaperId { get; set; }

        /// <summary>
        /// 试卷名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 试卷描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 总分
        /// </summary>
        public double TotalScore { get; set; }

        /// <summary>
        /// 及格分
        /// </summary>
        public double PassScore { get; set; }

        /// <summary>
        /// 考试时长(分钟)
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// 试卷类型
        /// </summary>
        public PaperType PaperType { get; set; }

        /// <summary>
        /// 随机组卷配置(JSON格式)
        /// </summary>
        public string RandomConfig { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        public int CreatorId { get; set; }

        /// <summary>
        /// 创建者导航属性
        /// </summary>
        public User Creator { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public PaperStatus Status { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
