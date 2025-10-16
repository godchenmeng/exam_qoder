using System;

namespace ExamSystem.Domain.Entities
{
    /// <summary>
    /// 答题记录实体
    /// </summary>
    public class AnswerRecord
    {
        /// <summary>
        /// 答题记录ID
        /// </summary>
        public int AnswerId { get; set; }

        /// <summary>
        /// 考试记录ID
        /// </summary>
        public int RecordId { get; set; }

        /// <summary>
        /// 考试记录导航属性
        /// </summary>
        public ExamRecord ExamRecord { get; set; }

        /// <summary>
        /// 题目ID
        /// </summary>
        public int QuestionId { get; set; }

        /// <summary>
        /// 题目导航属性
        /// </summary>
        public Question Question { get; set; }

        /// <summary>
        /// 考生答案
        /// </summary>
        public string UserAnswer { get; set; }

        /// <summary>
        /// 得分
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// 是否正确
        /// </summary>
        public bool IsCorrect { get; set; }

        /// <summary>
        /// 是否已评分
        /// </summary>
        public bool IsGraded { get; set; }

        /// <summary>
        /// 评分人ID
        /// </summary>
        public int? GraderId { get; set; }

        /// <summary>
        /// 评分人导航属性
        /// </summary>
        public User Grader { get; set; }

        /// <summary>
        /// 评分评语
        /// </summary>
        public string GradeComment { get; set; }

        /// <summary>
        /// 答题时间
        /// </summary>
        public DateTime? AnswerTime { get; set; }
    }
}
