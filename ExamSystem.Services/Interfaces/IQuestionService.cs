using System.Collections.Generic;
using System.Threading.Tasks;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.DTOs;
using ExamSystem.Domain.Enums;

namespace ExamSystem.Services.Interfaces
{
    /// <summary>
    /// 题目服务接口
    /// </summary>
    public interface IQuestionService
    {
        /// <summary>
        /// 根据ID获取题目
        /// </summary>
        Task<Question> GetQuestionByIdAsync(int questionId);

        /// <summary>
        /// 创建题目
        /// </summary>
        Task<Question> CreateQuestionAsync(Question question, List<Option> options = null);

        /// <summary>
        /// 更新题目
        /// </summary>
        Task<bool> UpdateQuestionAsync(Question question, List<Option> options = null);

        /// <summary>
        /// 删除题目
        /// </summary>
        Task<bool> DeleteQuestionAsync(int questionId);

        /// <summary>
        /// 根据题库获取题目列表
        /// </summary>
        Task<PagedResult<Question>> GetQuestionsByBankAsync(int bankId, int pageIndex, int pageSize);

        /// <summary>
        /// 搜索题目
        /// </summary>
        Task<PagedResult<Question>> SearchQuestionsAsync(
            int? bankId,
            QuestionType? questionType,
            Difficulty? difficulty,
            string keyword,
            int pageIndex,
            int pageSize);

        /// <summary>
        /// 验证题目
        /// </summary>
        ValidationResult ValidateQuestion(Question question, List<Option> options = null);

        /// <summary>
        /// 导入题目
        /// </summary>
        Task<ImportResult> ImportQuestionsAsync(int bankId, IEnumerable<Question> questions);

        /// <summary>
        /// 导出题目
        /// </summary>
        Task<byte[]> ExportQuestionsAsync(IEnumerable<int> questionIds, string format);

        /// <summary>
        /// 复制题目
        /// </summary>
        Task<Question> DuplicateQuestionAsync(int questionId);

        /// <summary>
        /// 获取题目及其选项
        /// </summary>
        Task<Question> GetQuestionWithOptionsAsync(int questionId);
        
        // 新增的方法
        /// <summary>
        /// 获取所有题库列表
        /// </summary>
        Task<IEnumerable<QuestionBank>> GetAllQuestionBanksAsync();

        Task<PagedResult<Question>> GetQuestionsByBankIdAsync(int bankId, int pageIndex, int pageSize);
    }
}