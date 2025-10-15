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
    /// 题目仓储实现
    /// </summary>
    public class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        public QuestionRepository(ExamSystemDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Question>> GetByBankIdAsync(int bankId)
        {
            return await _dbSet
                .Where(q => q.BankId == bankId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetByTypeAsync(QuestionType questionType)
        {
            return await _dbSet
                .Where(q => q.QuestionType == questionType)
                .ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetByDifficultyAsync(Difficulty difficulty)
        {
            return await _dbSet
                .Where(q => q.Difficulty == difficulty)
                .ToListAsync();
        }

        public async Task<PagedResult<Question>> SearchQuestionsAsync(
            int? bankId,
            QuestionType? questionType,
            Difficulty? difficulty,
            string keyword,
            int pageIndex,
            int pageSize)
        {
            var query = _dbSet.AsQueryable();

            // 应用筛选条件
            if (bankId.HasValue)
                query = query.Where(q => q.BankId == bankId.Value);

            if (questionType.HasValue)
                query = query.Where(q => q.QuestionType == questionType.Value);

            if (difficulty.HasValue)
                query = query.Where(q => q.Difficulty == difficulty.Value);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(q => q.Content.Contains(keyword));

            // 获取总数
            var totalCount = await query.CountAsync();

            // 分页
            var items = await query
                .OrderByDescending(q => q.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Question>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<Question> GetWithOptionsAsync(int questionId)
        {
            return await _dbSet
                .Include(q => q.QuestionBank)
                .Include("Options") // 需要在DbContext中配置导航属性
                .FirstOrDefaultAsync(q => q.QuestionId == questionId);
        }

        public async Task<IEnumerable<Question>> GetManyWithOptionsAsync(IEnumerable<int> questionIds)
        {
            return await _dbSet
                .Where(q => questionIds.Contains(q.QuestionId))
                .Include("Options")
                .ToListAsync();
        }
    }
}
