using System.Collections.Generic;
using System.Threading.Tasks;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Enums;
using ExamSystem.Domain.DTOs;

namespace ExamSystem.Repository.Interfaces
{
    /// <summary>
    /// 试卷仓储接口
    /// </summary>
    public interface IExamPaperRepository : IRepository<ExamPaper>
    {
        /// <summary>
        /// 获取试卷及其关联题目
        /// </summary>
        Task<ExamPaper> GetWithQuestionsAsync(int paperId);

        /// <summary>
        /// 根据创建者获取试卷列表
        /// </summary>
        Task<IEnumerable<ExamPaper>> GetByCreatorAsync(int creatorId);

        /// <summary>
        /// 根据状态获取试卷列表
        /// </summary>
        Task<IEnumerable<ExamPaper>> GetByStatusAsync(PaperStatus status);

        /// <summary>
        /// 获取当前激活的试卷列表
        /// </summary>
        Task<IEnumerable<ExamPaper>> GetActivePapersAsync();

        /// <summary>
        /// 搜索试卷（支持关键词、状态、创建者筛选）
        /// </summary>
        Task<PagedResult<ExamPaper>> SearchPapersAsync(
            string keyword, 
            PaperStatus? status, 
            int? creatorId, 
            int pageIndex, 
            int pageSize);
    }
}
