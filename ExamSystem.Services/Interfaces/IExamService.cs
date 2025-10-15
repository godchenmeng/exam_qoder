using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExamSystem.Domain.Entities;

namespace ExamSystem.Services.Interfaces
{
    /// <summary>
    /// 考试服务接口
    /// </summary>
    public interface IExamService
    {
        /// <summary>
        /// 开始考试
        /// </summary>
        Task<ExamRecord> StartExamAsync(int userId, int paperId);

        /// <summary>
        /// 恢复考试
        /// </summary>
        Task<ExamRecord> ResumeExamAsync(int recordId);

        /// <summary>
        /// 保存答案
        /// </summary>
        Task SaveAnswerAsync(int recordId, int questionId, string answer);

        /// <summary>
        /// 批量保存答案
        /// </summary>
        Task BatchSaveAnswersAsync(int recordId, Dictionary<int, string> answers);

        /// <summary>
        /// 提交考试
        /// </summary>
        Task<ExamRecord> SubmitExamAsync(int recordId);

        /// <summary>
        /// 获取考试进度
        /// </summary>
        Task<ExamProgress> GetExamProgressAsync(int recordId);

        /// <summary>
        /// 检查考试时间
        /// </summary>
        Task<bool> CheckExamTimeAsync(int recordId);

        /// <summary>
        /// 自动提交超时考试
        /// </summary>
        Task AutoSubmitTimeoutExamsAsync();

        /// <summary>
        /// 记录异常行为
        /// </summary>
        Task RecordAbnormalBehaviorAsync(int recordId, string behavior);
    }

    /// <summary>
    /// 考试进度信息
    /// </summary>
    public class ExamProgress
    {
        /// <summary>
        /// 总题数
        /// </summary>
        public int TotalQuestions { get; set; }

        /// <summary>
        /// 已答题数
        /// </summary>
        public int AnsweredQuestions { get; set; }

        /// <summary>
        /// 剩余时间（分钟）
        /// </summary>
        public int RemainingMinutes { get; set; }

        /// <summary>
        /// 是否超时
        /// </summary>
        public bool IsTimeout { get; set; }
    }
}
