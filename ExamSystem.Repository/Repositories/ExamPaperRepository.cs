using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Enums;
using ExamSystem.Domain.DTOs;
using ExamSystem.Repository.Context;
using ExamSystem.Repository.Interfaces;

namespace ExamSystem.Repository.Repositories
{
    /// <summary>
    /// 试卷仓储实现
    /// </summary>
    public class ExamPaperRepository : Repository<ExamPaper>, IExamPaperRepository
    {
        public ExamPaperRepository(ExamSystemDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 获取试卷及其关联题目
        /// </summary>
        public async Task<ExamPaper> GetWithQuestionsAsync(int paperId)
        {
            return await _context.ExamPapers
                .Include(p => p.Creator)
                .FirstOrDefaultAsync(p => p.PaperId == paperId);
        }

        /// <summary>
        /// 根据创建者获取试卷列表
        /// </summary>
        public async Task<IEnumerable<ExamPaper>> GetByCreatorAsync(int creatorId)
        {
            return await _context.ExamPapers
                .Include(p => p.Creator)
                .Where(p => p.CreatorId == creatorId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// 根据状态获取试卷列表
        /// </summary>
        public async Task<IEnumerable<ExamPaper>> GetByStatusAsync(PaperStatus status)
        {
            return await _context.ExamPapers
                .Include(p => p.Creator)
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// 获取当前激活的试卷列表
        /// </summary>
        public async Task<IEnumerable<ExamPaper>> GetActivePapersAsync()
        {
            var now = DateTime.Now;
            return await _context.ExamPapers
                .Include(p => p.Creator)
                .Where(p => p.Status == PaperStatus.Activated 
                    && (p.StartTime == null || p.StartTime <= now)
                    && (p.EndTime == null || p.EndTime >= now))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// 搜索试卷（支持关键词、状态、创建者筛选）
        /// </summary>
        public async Task<PagedResult<ExamPaper>> SearchPapersAsync(
            string keyword, 
            PaperStatus? status, 
            int? creatorId, 
            int pageIndex, 
            int pageSize)
        {
            var query = _context.ExamPapers
                .Include(p => p.Creator)
                .AsQueryable();

            // 关键词筛选
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p => p.Name.Contains(keyword) 
                    || p.Description.Contains(keyword));
            }

            // 状态筛选
            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            // 创建者筛选
            if (creatorId.HasValue)
            {
                query = query.Where(p => p.CreatorId == creatorId.Value);
            }

            // 总数
            var totalCount = await query.CountAsync();

            // 分页查询
            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ExamPaper>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
