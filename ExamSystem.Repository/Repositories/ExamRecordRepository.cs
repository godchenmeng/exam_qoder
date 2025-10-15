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
    /// 考试记录仓储实现
    /// </summary>
    public class ExamRecordRepository : Repository<ExamRecord>, IExamRecordRepository
    {
        public ExamRecordRepository(ExamSystemDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 获取考试记录及其详细信息
        /// </summary>
        public async Task<ExamRecord> GetWithDetailsAsync(int recordId)
        {
            return await _context.ExamRecords
                .Include(r => r.User)
                .Include(r => r.ExamPaper)
                .FirstOrDefaultAsync(r => r.RecordId == recordId);
        }

        /// <summary>
        /// 根据用户获取考试记录列表
        /// </summary>
        public async Task<IEnumerable<ExamRecord>> GetByUserIdAsync(int userId)
        {
            return await _context.ExamRecords
                .Include(r => r.User)
                .Include(r => r.ExamPaper)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// 根据试卷获取考试记录列表
        /// </summary>
        public async Task<IEnumerable<ExamRecord>> GetByPaperIdAsync(int paperId)
        {
            return await _context.ExamRecords
                .Include(r => r.User)
                .Include(r => r.ExamPaper)
                .Where(r => r.PaperId == paperId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// 获取用户特定试卷的考试记录
        /// </summary>
        public async Task<ExamRecord> GetUserExamRecordAsync(int userId, int paperId)
        {
            return await _context.ExamRecords
                .Include(r => r.User)
                .Include(r => r.ExamPaper)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.PaperId == paperId);
        }

        /// <summary>
        /// 获取未完成的考试记录
        /// </summary>
        public async Task<IEnumerable<ExamRecord>> GetUnfinishedRecordsAsync(int userId)
        {
            return await _context.ExamRecords
                .Include(r => r.User)
                .Include(r => r.ExamPaper)
                .Where(r => r.UserId == userId && r.Status == ExamStatus.InProgress)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// 获取待评分的考试记录
        /// </summary>
        public async Task<IEnumerable<ExamRecord>> GetRecordsNeedGradingAsync()
        {
            return await _context.ExamRecords
                .Include(r => r.User)
                .Include(r => r.ExamPaper)
                .Where(r => r.Status == ExamStatus.Submitted)
                .OrderBy(r => r.SubmitTime)
                .ToListAsync();
        }
    }
}
