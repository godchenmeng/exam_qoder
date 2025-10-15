using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.DTOs;
using ExamSystem.Repository.Context;
using ExamSystem.Repository.Interfaces;

namespace ExamSystem.Repository.Repositories
{
    /// <summary>
    /// 题库仓储实现
    /// </summary>
    public class QuestionBankRepository : Repository<QuestionBank>, IQuestionBankRepository
    {
        public QuestionBankRepository(ExamSystemDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 根据创建者获取题库列表
        /// </summary>
        public async Task<IEnumerable<QuestionBank>> GetByCreatorAsync(int creatorId)
        {
            return await _context.QuestionBanks
                .Include(b => b.Creator)
                .Where(b => b.CreatorId == creatorId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// 获取公开题库列表
        /// </summary>
        public async Task<IEnumerable<QuestionBank>> GetPublicBanksAsync()
        {
            return await _context.QuestionBanks
                .Include(b => b.Creator)
                .Where(b => b.IsPublic)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// 获取题库及其题目
        /// </summary>
        public async Task<QuestionBank> GetBankWithQuestionsAsync(int bankId)
        {
            return await _context.QuestionBanks
                .Include(b => b.Creator)
                .FirstOrDefaultAsync(b => b.BankId == bankId);
        }

        /// <summary>
        /// 获取题库统计信息
        /// </summary>
        public async Task<BankStatistics> GetBankStatisticsAsync(int bankId)
        {
            var bank = await _context.QuestionBanks
                .FirstOrDefaultAsync(b => b.BankId == bankId);

            if (bank == null)
                return null;

            var questions = await _context.Questions
                .Where(q => q.BankId == bankId)
                .ToListAsync();

            var statistics = new BankStatistics
            {
                BankId = bank.BankId,
                BankName = bank.Name,
                TotalQuestions = questions.Count
            };

            // 统计各题型数量
            var typeGroups = questions.GroupBy(q => q.QuestionType);
            foreach (var group in typeGroups)
            {
                statistics.QuestionTypeDistribution[group.Key.ToString()] = group.Count();
            }

            // 统计各难度数量
            var difficultyGroups = questions.GroupBy(q => q.Difficulty);
            foreach (var group in difficultyGroups)
            {
                statistics.DifficultyDistribution[group.Key.ToString()] = group.Count();
            }

            return statistics;
        }
    }
}
