using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Enums;
using ExamSystem.Repository.Interfaces;
using ExamSystem.Services.Interfaces;
using ExamSystem.Infrastructure.Utils;

namespace ExamSystem.Services.Implementations
{
    /// <summary>
    /// 考试服务实现
    /// </summary>
    public class ExamService : IExamService
    {
        private readonly IExamRecordRepository _examRecordRepository;
        private readonly IAnswerRecordRepository _answerRecordRepository;
        private readonly IExamPaperRepository _paperRepository;
        private readonly IRepository<PaperQuestion> _paperQuestionRepository;
        private readonly IGradingService _gradingService;

        public ExamService(
            IExamRecordRepository examRecordRepository,
            IAnswerRecordRepository answerRecordRepository,
            IExamPaperRepository paperRepository,
            IRepository<PaperQuestion> paperQuestionRepository,
            IGradingService gradingService)
        {
            _examRecordRepository = examRecordRepository ?? throw new ArgumentNullException(nameof(examRecordRepository));
            _answerRecordRepository = answerRecordRepository ?? throw new ArgumentNullException(nameof(answerRecordRepository));
            _paperRepository = paperRepository ?? throw new ArgumentNullException(nameof(paperRepository));
            _paperQuestionRepository = paperQuestionRepository ?? throw new ArgumentNullException(nameof(paperQuestionRepository));
            _gradingService = gradingService ?? throw new ArgumentNullException(nameof(gradingService));
        }

        /// <summary>
        /// 开始考试
        /// </summary>
        public async Task<ExamRecord> StartExamAsync(int userId, int paperId)
        {
            // 检查试卷是否存在且已激活
            var paper = await _paperRepository.GetByIdAsync(paperId);
            if (paper == null)
                throw new InvalidOperationException("试卷不存在");

            if (paper.Status != PaperStatus.Active)
                throw new InvalidOperationException("试卷未激活，无法开始考试");

            // 检查试卷时间范围
            var now = DateTime.Now;
            if (paper.StartTime.HasValue && now < paper.StartTime.Value)
                throw new InvalidOperationException("考试尚未开始");

            if (paper.EndTime.HasValue && now > paper.EndTime.Value)
                throw new InvalidOperationException("考试已结束");

            // 检查是否已有未完成的考试记录
            var existingRecord = await _examRecordRepository.GetUserExamRecordAsync(userId, paperId);
            if (existingRecord != null && existingRecord.Status == ExamStatus.InProgress)
                throw new InvalidOperationException("您已有正在进行的考试，请继续完成");

            // 创建考试记录
            var examRecord = new ExamRecord
            {
                UserId = userId,
                PaperId = paperId,
                StartTime = DateTime.Now,
                Status = ExamStatus.InProgress,
                CreatedAt = DateTime.Now
            };

            await _examRecordRepository.AddAsync(examRecord);
            await _examRecordRepository.SaveChangesAsync();

            return examRecord;
        }

        /// <summary>
        /// 恢复考试
        /// </summary>
        public async Task<ExamRecord> ResumeExamAsync(int recordId)
        {
            var record = await _examRecordRepository.GetWithDetailsAsync(recordId);
            if (record == null)
                throw new InvalidOperationException("考试记录不存在");

            if (record.Status != ExamStatus.InProgress)
                throw new InvalidOperationException("该考试已结束，无法恢复");

            // 检查是否超时
            var isTimeout = await CheckExamTimeAsync(recordId);
            if (isTimeout)
            {
                // 自动提交超时考试
                await SubmitExamAsync(recordId);
                throw new InvalidOperationException("考试已超时，系统已自动提交");
            }

            return record;
        }

        /// <summary>
        /// 保存答案
        /// </summary>
        public async Task SaveAnswerAsync(int recordId, int questionId, string answer)
        {
            var record = await _examRecordRepository.GetByIdAsync(recordId);
            if (record == null)
                throw new InvalidOperationException("考试记录不存在");

            if (record.Status != ExamStatus.InProgress)
                throw new InvalidOperationException("考试已结束，无法保存答案");

            // 检查是否超时
            var isTimeout = await CheckExamTimeAsync(recordId);
            if (isTimeout)
                throw new InvalidOperationException("考试已超时");

            // 查找或创建答题记录
            var answerRecord = await _answerRecordRepository.GetAnswerRecordAsync(recordId, questionId);
            
            if (answerRecord == null)
            {
                answerRecord = new AnswerRecord
                {
                    RecordId = recordId,
                    QuestionId = questionId,
                    UserAnswer = answer,
                    IsGraded = false,
                    AnswerTime = DateTime.Now
                };
                await _answerRecordRepository.AddAsync(answerRecord);
            }
            else
            {
                answerRecord.UserAnswer = answer;
                answerRecord.AnswerTime = DateTime.Now;
                await _answerRecordRepository.UpdateAsync(answerRecord);
            }

            await _answerRecordRepository.SaveChangesAsync();
        }

        /// <summary>
        /// 批量保存答案
        /// </summary>
        public async Task BatchSaveAnswersAsync(int recordId, Dictionary<int, string> answers)
        {
            var record = await _examRecordRepository.GetByIdAsync(recordId);
            if (record == null)
                throw new InvalidOperationException("考试记录不存在");

            if (record.Status != ExamStatus.InProgress)
                throw new InvalidOperationException("考试已结束，无法保存答案");

            var answerRecords = new List<AnswerRecord>();
            
            foreach (var kvp in answers)
            {
                answerRecords.Add(new AnswerRecord
                {
                    RecordId = recordId,
                    QuestionId = kvp.Key,
                    UserAnswer = kvp.Value,
                    IsGraded = false,
                    AnswerTime = DateTime.Now
                });
            }

            await _answerRecordRepository.BatchSaveAnswersAsync(answerRecords);
        }

        /// <summary>
        /// 提交考试
        /// </summary>
        public async Task<ExamRecord> SubmitExamAsync(int recordId)
        {
            var record = await _examRecordRepository.GetWithDetailsAsync(recordId);
            if (record == null)
                throw new InvalidOperationException("考试记录不存在");

            if (record.Status != ExamStatus.InProgress)
                throw new InvalidOperationException("考试已提交");

            // 更新考试记录状态
            record.Status = ExamStatus.Submitted;
            record.SubmitTime = DateTime.Now;
            record.EndTime = DateTime.Now;

            await _examRecordRepository.UpdateAsync(record);
            await _examRecordRepository.SaveChangesAsync();

            // 触发自动评分
            try
            {
                await _gradingService.AutoGradeObjectiveQuestionsAsync(recordId);
                
                // 计算总分（如果没有主观题，直接计算总分）
                var subjectiveAnswers = await _answerRecordRepository.GetSubjectiveAnswersAsync(recordId);
                if (!subjectiveAnswers.Any())
                {
                    await _gradingService.CalculateTotalScoreAsync(recordId);
                }
            }
            catch (Exception ex)
            {
                // 记录评分错误，但不影响提交
                Console.WriteLine($"自动评分失败: {ex.Message}");
            }

            return record;
        }

        /// <summary>
        /// 获取考试进度
        /// </summary>
        public async Task<ExamProgress> GetExamProgressAsync(int recordId)
        {
            var record = await _examRecordRepository.GetWithDetailsAsync(recordId);
            if (record == null)
                throw new InvalidOperationException("考试记录不存在");

            // 获取试卷题目数
            var paperQuestions = await _paperQuestionRepository.FindAsync(pq => pq.PaperId == record.PaperId);
            var totalQuestions = paperQuestions.Count();

            // 获取已答题数
            var answerRecords = await _answerRecordRepository.GetByExamRecordAsync(recordId);
            var answeredQuestions = answerRecords.Count(a => !string.IsNullOrWhiteSpace(a.UserAnswer));

            // 计算剩余时间
            var elapsed = DateTime.Now - record.StartTime;
            var duration = record.ExamPaper.Duration;
            var remainingMinutes = Math.Max(0, duration - (int)elapsed.TotalMinutes);
            var isTimeout = elapsed.TotalMinutes > duration;

            return new ExamProgress
            {
                TotalQuestions = totalQuestions,
                AnsweredQuestions = answeredQuestions,
                RemainingMinutes = remainingMinutes,
                IsTimeout = isTimeout
            };
        }

        /// <summary>
        /// 检查考试时间
        /// </summary>
        public async Task<bool> CheckExamTimeAsync(int recordId)
        {
            var record = await _examRecordRepository.GetWithDetailsAsync(recordId);
            if (record == null)
                return false;

            var elapsed = DateTime.Now - record.StartTime;
            return elapsed.TotalMinutes > record.ExamPaper.Duration;
        }

        /// <summary>
        /// 自动提交超时考试
        /// </summary>
        public async Task AutoSubmitTimeoutExamsAsync()
        {
            var allRecords = await _examRecordRepository.FindAsync(r => r.Status == ExamStatus.InProgress);

            foreach (var record in allRecords)
            {
                var isTimeout = await CheckExamTimeAsync(record.RecordId);
                if (isTimeout)
                {
                    record.Status = ExamStatus.Timeout;
                    record.SubmitTime = DateTime.Now;
                    record.EndTime = DateTime.Now;

                    await _examRecordRepository.UpdateAsync(record);
                    
                    // 触发自动评分
                    try
                    {
                        await _gradingService.AutoGradeObjectiveQuestionsAsync(record.RecordId);
                        
                        var subjectiveAnswers = await _answerRecordRepository.GetSubjectiveAnswersAsync(record.RecordId);
                        if (!subjectiveAnswers.Any())
                        {
                            await _gradingService.CalculateTotalScoreAsync(record.RecordId);
                        }
                    }
                    catch
                    {
                        // 忽略评分错误
                    }
                }
            }

            await _examRecordRepository.SaveChangesAsync();
        }

        /// <summary>
        /// 记录异常行为
        /// </summary>
        public async Task RecordAbnormalBehaviorAsync(int recordId, string behavior)
        {
            var record = await _examRecordRepository.GetByIdAsync(recordId);
            if (record == null)
                return;

            var behaviors = new List<string>();
            
            if (!string.IsNullOrWhiteSpace(record.AbnormalBehaviors))
            {
                try
                {
                    behaviors = JsonHelper.Deserialize<List<string>>(record.AbnormalBehaviors);
                }
                catch
                {
                    behaviors = new List<string>();
                }
            }

            behaviors.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {behavior}");
            record.AbnormalBehaviors = JsonHelper.Serialize(behaviors);

            await _examRecordRepository.UpdateAsync(record);
            await _examRecordRepository.SaveChangesAsync();
        }
    }
}
