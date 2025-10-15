using System.Collections.Generic;
using System.Threading.Tasks;
using ExamSystem.Domain.Entities;

namespace ExamSystem.Repository.Interfaces
{
    /// <summary>
    /// 考试记录仓储接口
    /// </summary>
    public interface IExamRecordRepository : IRepository<ExamRecord>
    {
        /// <summary>
        /// 获取考试记录及其详细信息
        /// </summary>
        Task<ExamRecord> GetWithDetailsAsync(int recordId);

        /// <summary>
        /// 根据用户获取考试记录列表
        /// </summary>
        Task<IEnumerable<ExamRecord>> GetByUserIdAsync(int userId);

        /// <summary>
        /// 根据试卷获取考试记录列表
        /// </summary>
        Task<IEnumerable<ExamRecord>> GetByPaperIdAsync(int paperId);

        /// <summary>
        /// 获取用户特定试卷的考试记录
        /// </summary>
        Task<ExamRecord> GetUserExamRecordAsync(int userId, int paperId);

        /// <summary>
        /// 获取未完成的考试记录
        /// </summary>
        Task<IEnumerable<ExamRecord>> GetUnfinishedRecordsAsync(int userId);

        /// <summary>
        /// 获取待评分的考试记录
        /// </summary>
        Task<IEnumerable<ExamRecord>> GetRecordsNeedGradingAsync();
    }
}
