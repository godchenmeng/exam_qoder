using System.Collections.Generic;
using System.Threading.Tasks;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.DTOs;

namespace ExamSystem.Repository.Interfaces
{
    /// <summary>
    /// 题库仓储接口
    /// </summary>
    public interface IQuestionBankRepository : IRepository<QuestionBank>
    {
        /// <summary>
        /// 根据创建者获取题库列表
        /// </summary>
        Task<IEnumerable<QuestionBank>> GetByCreatorAsync(int creatorId);

        /// <summary>
        /// 获取公开题库列表
        /// </summary>
        Task<IEnumerable<QuestionBank>> GetPublicBanksAsync();

        /// <summary>
        /// 获取题库及其题目
        /// </summary>
        Task<QuestionBank> GetBankWithQuestionsAsync(int bankId);

        /// <summary>
        /// 获取题库统计信息
        /// </summary>
        Task<BankStatistics> GetBankStatisticsAsync(int bankId);
    }
}
