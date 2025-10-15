using System.Collections.Generic;
using System.Threading.Tasks;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.DTOs;

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
        /// 删除试卷
        /// </summary>
        Task DeletePaperAsync(int paperId);

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
