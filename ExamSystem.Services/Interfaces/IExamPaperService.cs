using System.Collections.Generic;
using System.Threading.Tasks;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.DTOs;
using ExamSystem.Domain.Enums;

namespace ExamSystem.Services.Interfaces
{
    /// <summary>
    /// 试卷服务接口
    /// </summary>
    public interface IExamPaperService
    {
        /// <summary>
        /// 创建固定试卷
        /// </summary>
        Task<ExamPaper> CreateFixedPaperAsync(ExamPaper paper, List<PaperQuestion> questions);

        /// <summary>
        /// 创建随机试卷
        /// </summary>
        Task<ExamPaper> CreateRandomPaperAsync(ExamPaper paper, RandomPaperConfig config);

        /// <summary>
        /// 创建混合试卷
        /// </summary>
        Task<ExamPaper> CreateMixedPaperAsync(ExamPaper paper, MixedPaperConfig config);

        /// <summary>
        /// 获取试卷详情
        /// </summary>
        Task<ExamPaper> GetPaperWithQuestionsAsync(int paperId);

        /// <summary>
        /// 获取试卷的题目列表
        /// </summary>
        Task<List<PaperQuestion>> GetPaperQuestionsAsync(int paperId);

        /// <summary>
        /// 更新试卷
        /// </summary>
        Task UpdatePaperAsync(ExamPaper paper);

        /// <summary>
        /// 获取所有试卷列表
        /// </summary>
        /// <returns>所有试卷集合，按创建时间倒序排列</returns>
        Task<List<ExamPaper>> GetAllExamPapersAsync();

        /// <summary>
        /// 根据条件筛选试卷列表
        /// </summary>
        /// <param name="status">试卷状态过滤器（可选）</param>
        /// <param name="type">试卷类型过滤器（可选）</param>
        /// <param name="keyword">关键词搜索（可选，模糊匹配名称和描述）</param>
        /// <returns>符合条件的试卷集合</returns>
        Task<List<ExamPaper>> GetExamPapersAsync(PaperStatus? status, PaperType? type, string? keyword);

        /// <summary>
        /// 删除试卷
        /// </summary>
        /// <param name="paperId">试卷ID</param>
        Task DeletePaperAsync(int paperId);

        /// <summary>
        /// 删除试卷（别名方法，保持兼容性）
        /// </summary>
        /// <param name="paperId">试卷ID</param>
        Task DeleteExamPaperAsync(int paperId);

        /// <summary>
        /// 激活试卷
        /// </summary>
        Task ActivatePaperAsync(int paperId);

        /// <summary>
        /// 归档试卷
        /// </summary>
        Task ArchivePaperAsync(int paperId);

        /// <summary>
        /// 复制试卷
        /// </summary>
        Task<ExamPaper> DuplicatePaperAsync(int paperId);

        /// <summary>
        /// 预览试卷
        /// </summary>
        Task<ExamPaper> PreviewPaperAsync(int paperId);

        /// <summary>
        /// 验证试卷
        /// </summary>
        ValidationResult ValidatePaper(ExamPaper paper);

        /// <summary>
        /// 搜索试卷
        /// </summary>
        Task<PagedResult<ExamPaper>> SearchPapersAsync(string keyword, int? creatorId, int pageIndex, int pageSize);
    }
}
