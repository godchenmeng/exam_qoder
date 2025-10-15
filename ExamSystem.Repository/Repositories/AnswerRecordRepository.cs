using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Enums;
using ExamSystem.Repository.Context;
using ExamSystem.Repository.Interfaces;

namespace ExamSystem.Repository.Repositories
{
    /// <summary>
    /// 答题记录仓储实现
    /// </summary>
    public class AnswerRecordRepository : Repository<AnswerRecord>, IAnswerRecordRepository
    {
        public AnswerRecordRepository(ExamSystemDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 获取考试记录的所有答题记录
        /// </summary>
        public async Task<IEnumerable<AnswerRecord>> GetByExamRecordAsync(int examRecordId)
        {
            return await _context.AnswerRecords
                .Include(a => a.Question)
                .Include(a => a.Grader)
                .Where(a => a.RecordId == examRecordId)
                .OrderBy(a => a.QuestionId)
                .ToListAsync();
        }

        /// <summary>
        /// 获取特定题目的答题记录
        /// </summary>
        public async Task<AnswerRecord> GetAnswerRecordAsync(int examRecordId, int questionId)
        {
            return await _context.AnswerRecords
                .Include(a => a.Question)
                .Include(a => a.Grader)
                .FirstOrDefaultAsync(a => a.RecordId == examRecordId && a.QuestionId == questionId);
        }

        /// <summary>
        /// 批量保存答题记录
        /// </summary>
        public async Task<int> BatchSaveAnswersAsync(IEnumerable<AnswerRecord> answerRecords)
        {
            if (answerRecords == null || !answerRecords.Any())
                return 0;

            foreach (var record in answerRecords)
            {
                var existing = await _context.AnswerRecords
                    .FirstOrDefaultAsync(a => a.RecordId == record.RecordId && a.QuestionId == record.QuestionId);

                if (existing != null)
                {
                    // 更新现有记录
                    existing.UserAnswer = record.UserAnswer;
                    existing.AnswerTime = record.AnswerTime ?? DateTime.Now;
                    _context.AnswerRecords.Update(existing);
                }
                else
                {
                    // 添加新记录
                    record.AnswerTime = record.AnswerTime ?? DateTime.Now;
                    await _context.AnswerRecords.AddAsync(record);
                }
            }

            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 获取主观题答题记录
        /// </summary>
        public async Task<IEnumerable<AnswerRecord>> GetSubjectiveAnswersAsync(int examRecordId)
        {
            return await _context.AnswerRecords
                .Include(a => a.Question)
                .Include(a => a.Grader)
                .Where(a => a.RecordId == examRecordId 
                    && (a.Question.QuestionType == QuestionType.ShortAnswer 
                        || a.Question.QuestionType == QuestionType.Essay))
                .OrderBy(a => a.QuestionId)
                .ToListAsync();
        }
    }
}
