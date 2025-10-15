using System.Collections.Generic;
using System.Threading.Tasks;
using ExamSystem.Domain.Entities;

namespace ExamSystem.Repository.Interfaces
{
    /// <summary>
    /// 答题记录仓储接口
    /// </summary>
    public interface IAnswerRecordRepository : IRepository<AnswerRecord>
    {
        /// <summary>
        /// 获取考试记录的所有答题记录
        /// </summary>
        Task<IEnumerable<AnswerRecord>> GetByExamRecordAsync(int examRecordId);

        /// <summary>
        /// 获取特定题目的答题记录
        /// </summary>
        Task<AnswerRecord> GetAnswerRecordAsync(int examRecordId, int questionId);

        /// <summary>
        /// 批量保存答题记录
        /// </summary>
        Task<int> BatchSaveAnswersAsync(IEnumerable<AnswerRecord> answerRecords);

        /// <summary>
        /// 获取主观题答题记录
        /// </summary>
        Task<IEnumerable<AnswerRecord>> GetSubjectiveAnswersAsync(int examRecordId);
    }
}
