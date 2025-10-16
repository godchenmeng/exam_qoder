using System.Collections.Generic;
using System.Threading.Tasks;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.DTOs;

namespace ExamSystem.Services.Interfaces
{
    /// <summary>
    /// 评分服务接口
    /// </summary>
    public interface IGradingService
    {
        /// <summary>
        /// 自动评分客观题
        /// </summary>
        Task AutoGradeObjectiveQuestionsAsync(int recordId);

        /// <summary>
        /// 人工评分主观题
        /// </summary>
        Task ManualGradeSubjectiveQuestionAsync(int answerRecordId, double score, string comment, int graderId);

        /// <summary>
        /// 批量评分
        /// </summary>
        Task BatchGradeSubjectiveQuestionsAsync(List<GradeItem> gradeItems, int graderId);

        /// <summary>
        /// 重新评分
        /// </summary>
        Task RegradePaperAsync(int recordId);

        /// <summary>
        /// 获取待评分列表
        /// </summary>
        Task<IEnumerable<ExamRecord>> GetPendingGradingRecordsAsync();

        /// <summary>
        /// 计算总分
        /// </summary>
        Task CalculateTotalScoreAsync(int recordId);
    }
}
