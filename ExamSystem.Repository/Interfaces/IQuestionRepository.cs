using System.Collections.Generic;
using System.Threading.Tasks;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Enums;
using ExamSystem.Domain.DTOs;

namespace ExamSystem.Repository.Interfaces
{
    /// <summary>
    /// 题目仓储接口
    /// </summary>
    public interface IQuestionRepository : IRepository<Question>
    {
        /// <summary>
        /// 根据题库ID获取题目列表
        /// </summary>
        Task<IEnumerable<Question>> GetByBankIdAsync(int bankId);

        /// <summary>
        /// 根据题型获取题目列表
        /// </summary>
        Task<IEnumerable<Question>> GetByTypeAsync(QuestionType questionType);

        /// <summary>
        /// 根据难度获取题目列表
        /// </summary>
        Task<IEnumerable<Question>> GetByDifficultyAsync(Difficulty difficulty);

        /// <summary>
        /// 搜索题目(支持关键词、题库、题型、难度筛选)
        /// </summary>
        Task<PagedResult<Question>> SearchQuestionsAsync(
            int? bankId,
            QuestionType? questionType,
            Difficulty? difficulty,
            string keyword,
            int pageIndex,
            int pageSize);

        /// <summary>
        /// 获取题目及其选项
        /// </summary>
        Task<Question> GetWithOptionsAsync(int questionId);

        /// <summary>
        /// 批量获取题目及其选项
        /// </summary>
        Task<IEnumerable<Question>> GetManyWithOptionsAsync(IEnumerable<int> questionIds);

        /// <summary>
        /// 根据题库ID、题型和难度获取题目列表（用于随机组卷）
        /// </summary>
        /// <param name="bankId">题库ID</param>
        /// <param name="questionType">题型（可选）</param>
        /// <param name="difficulty">难度（可选）</param>
        /// <returns>符合条件的题目集合</returns>
        Task<IEnumerable<Question>> GetQuestionsByBankAndTypeAsync(
            int bankId,
            QuestionType? questionType = null,
            Difficulty? difficulty = null);
    }
}
