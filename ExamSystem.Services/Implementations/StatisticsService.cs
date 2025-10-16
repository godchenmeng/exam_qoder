using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Enums;
using ExamSystem.Domain.DTOs;
using ExamSystem.Repository.Interfaces;
using ExamSystem.Services.Interfaces;

namespace ExamSystem.Services.Implementations
{
    /// <summary>
    /// 统计分析服务实现
    /// </summary>
    public class StatisticsService : IStatisticsService
    {
        private readonly IExamRecordRepository _examRecordRepository;
        private readonly IAnswerRecordRepository _answerRecordRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IQuestionBankRepository _questionBankRepository;
        private readonly IExamPaperRepository _examPaperRepository;

        public StatisticsService(
            IExamRecordRepository examRecordRepository,
            IAnswerRecordRepository answerRecordRepository,
            IQuestionRepository questionRepository,
            IUserRepository userRepository,
            IQuestionBankRepository questionBankRepository,
            IExamPaperRepository examPaperRepository)
        {
            _examRecordRepository = examRecordRepository ?? throw new ArgumentNullException(nameof(examRecordRepository));
            _answerRecordRepository = answerRecordRepository ?? throw new ArgumentNullException(nameof(answerRecordRepository));
            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _questionBankRepository = questionBankRepository ?? throw new ArgumentNullException(nameof(questionBankRepository));
            _examPaperRepository = examPaperRepository ?? throw new ArgumentNullException(nameof(examPaperRepository));
        }

        /// <summary>
        /// 获取试卷统计
        /// </summary>
        public async Task<PaperStatistics> GetPaperStatisticsAsync(int paperId)
        {
            var records = await _examRecordRepository.GetByPaperIdAsync(paperId);
            var recordsList = records.ToList();

            if (!recordsList.Any())
            {
                return new PaperStatistics
                {
                    PaperId = paperId,
                    PaperName = recordsList.FirstOrDefault()?.ExamPaper?.Name ?? "未知试卷"
                };
            }

            var gradedRecords = recordsList.Where(r => r.Status == ExamStatus.Graded && r.TotalScore > 0).ToList();

            var statistics = new PaperStatistics
            {
                PaperId = paperId,
                PaperName = recordsList.First().ExamPaper?.Name ?? "未知试卷",
                TotalExams = recordsList.Count,
                SubmittedCount = recordsList.Count(r => r.Status == ExamStatus.Submitted || r.Status == ExamStatus.Graded),
                GradedCount = gradedRecords.Count
            };

            if (gradedRecords.Any())
            {
                var scores = gradedRecords.Select(r => r.TotalScore).ToList();
                var passScore = recordsList.First().ExamPaper?.PassScore ?? 60;

                statistics.PassedCount = gradedRecords.Count(r => r.IsPassed == true);
                statistics.PassRate = statistics.GradedCount > 0 
                    ? (decimal)statistics.PassedCount / statistics.GradedCount * 100 
                    : 0;
                statistics.AverageScore = scores.Average();
                statistics.HighestScore = scores.Max();
                statistics.LowestScore = scores.Min();

                // 分数段分布
                foreach (var score in scores)
                {
                    if (score < 60)
                        statistics.ScoreDistribution["0-60"]++;
                    else if (score < 70)
                        statistics.ScoreDistribution["60-70"]++;
                    else if (score < 80)
                        statistics.ScoreDistribution["70-80"]++;
                    else if (score < 90)
                        statistics.ScoreDistribution["80-90"]++;
                    else
                        statistics.ScoreDistribution["90-100"]++;
                }
            }

            return statistics;
        }

        /// <summary>
        /// 获取学生成绩统计
        /// </summary>
        public async Task<StudentScoreStatistics> GetStudentScoreStatisticsAsync(int userId)
        {
            var records = await _examRecordRepository.GetByUserIdAsync(userId);
            var gradedRecords = records.Where(r => r.Status == ExamStatus.Graded && r.TotalScore > 0).ToList();

            var statistics = new StudentScoreStatistics
            {
                UserId = userId,
                Username = gradedRecords.FirstOrDefault()?.User?.Username ?? "未知用户",
                TotalExams = gradedRecords.Count,
                PassedExams = gradedRecords.Count(r => r.IsPassed == true)
            };

            if (gradedRecords.Any())
            {
                var scores = gradedRecords.Select(r => r.TotalScore).ToList();
                statistics.AverageScore = scores.Average();
                statistics.HighestScore = scores.Max();
                statistics.LowestScore = scores.Min();
            }

            return statistics;
        }

        /// <summary>
        /// 获取题目分析
        /// </summary>
        public async Task<QuestionAnalysis> GetQuestionAnalysisAsync(int questionId, int paperId)
        {
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
                throw new InvalidOperationException("题目不存在");

            // 获取该试卷所有考试记录
            var examRecords = await _examRecordRepository.GetByPaperIdAsync(paperId);
            var recordIds = examRecords.Select(r => r.RecordId).ToList();

            // 获取该题的所有答题记录
            var allAnswers = new List<AnswerRecord>();
            foreach (var recordId in recordIds)
            {
                var answers = await _answerRecordRepository.GetByExamRecordAsync(recordId);
                var questionAnswer = answers.FirstOrDefault(a => a.QuestionId == questionId);
                if (questionAnswer != null)
                    allAnswers.Add(questionAnswer);
            }

            var analysis = new QuestionAnalysis
            {
                QuestionId = questionId,
                QuestionContent = question.Content,
                TotalAnswers = allAnswers.Count,
                CorrectAnswers = allAnswers.Count(a => a.IsCorrect == true),
                FullScore = question.DefaultScore
            };

            if (analysis.TotalAnswers > 0)
            {
                analysis.CorrectRate = (decimal)analysis.CorrectAnswers / analysis.TotalAnswers * 100;
                
                var scoredAnswers = allAnswers.Where(a => a.Score > 0).ToList();
                if (scoredAnswers.Any())
                {
                    analysis.AverageScore = scoredAnswers.Average(a => a.Score);
                    analysis.ScoreRate = analysis.AverageScore / analysis.FullScore * 100;
                }

                // 计算区分度（高分组正确率 - 低分组正确率）
                var totalRecords = examRecords.Where(r => r.TotalScore > 0).OrderByDescending(r => r.TotalScore).ToList();
                if (totalRecords.Count >= 10)
                {
                    var topCount = Math.Max(1, totalRecords.Count / 3);
                    var topRecords = totalRecords.Take(topCount).ToList();
                    var bottomRecords = totalRecords.TakeLast(topCount).ToList();

                    var topAnswers = allAnswers.Where(a => topRecords.Any(r => r.RecordId == a.RecordId)).ToList();
                    var bottomAnswers = allAnswers.Where(a => bottomRecords.Any(r => r.RecordId == a.RecordId)).ToList();

                    var topCorrectRate = topAnswers.Any() 
                        ? (decimal)topAnswers.Count(a => a.IsCorrect == true) / topAnswers.Count * 100 
                        : 0;
                    var bottomCorrectRate = bottomAnswers.Any() 
                        ? (decimal)bottomAnswers.Count(a => a.IsCorrect == true) / bottomAnswers.Count * 100 
                        : 0;

                    analysis.Discrimination = topCorrectRate - bottomCorrectRate;
                }
            }

            return analysis;
        }

        /// <summary>
        /// 获取班级排名
        /// </summary>
        public async Task<List<StudentRanking>> GetClassRankingAsync(int paperId)
        {
            var records = await _examRecordRepository.GetByPaperIdAsync(paperId);
            var gradedRecords = records
                .Where(r => r.Status == ExamStatus.Graded && r.TotalScore > 0)
                .OrderByDescending(r => r.TotalScore)
                .ToList();

            var rankings = new List<StudentRanking>();
            int rank = 1;

            foreach (var record in gradedRecords)
            {
                rankings.Add(new StudentRanking
                {
                    Rank = rank++,
                    UserId = record.UserId,
                    Username = record.User?.Username ?? "未知",
                    RealName = record.User?.RealName ?? "未知",
                    TotalScore = record.TotalScore,
                    IsPassed = record.IsPassed ?? false
                });
            }

            return rankings;
        }

        /// <summary>
        /// 获取错题统计
        /// </summary>
        public async Task<List<WrongQuestion>> GetWrongQuestionsAsync(int userId)
        {
            var examRecords = await _examRecordRepository.GetByUserIdAsync(userId);
            var wrongQuestionsDict = new Dictionary<int, WrongQuestion>();

            foreach (var record in examRecords)
            {
                var answers = await _answerRecordRepository.GetByExamRecordAsync(record.RecordId);
                var wrongAnswers = answers.Where(a => a.IsCorrect == false).ToList();

                foreach (var answer in wrongAnswers)
                {
                    if (wrongQuestionsDict.ContainsKey(answer.QuestionId))
                    {
                        wrongQuestionsDict[answer.QuestionId].WrongCount++;
                    }
                    else
                    {
                        var question = await _questionRepository.GetByIdAsync(answer.QuestionId);
                        wrongQuestionsDict[answer.QuestionId] = new WrongQuestion
                        {
                            QuestionId = answer.QuestionId,
                            QuestionContent = question?.Content ?? "未知题目",
                            QuestionType = question?.QuestionType.ToString() ?? "未知",
                            UserAnswer = answer.UserAnswer,
                            CorrectAnswer = question?.Answer ?? "未知",
                            WrongCount = 1
                        };
                    }
                }
            }

            return wrongQuestionsDict.Values
                .OrderByDescending(w => w.WrongCount)
                .ToList();
        }

        /// <summary>
        /// 获取系统统计数据
        /// </summary>
        public async Task<SystemStatistics> GetSystemStatisticsAsync()
        {
            // 获取各类统计数据
            var users = await _userRepository.GetAllAsync();
            var banks = await _questionBankRepository.GetAllAsync();
            var questions = await _questionRepository.GetAllAsync();
            var papers = await _examPaperRepository.GetAllAsync();
            var examRecords = await _examRecordRepository.GetAllAsync();

            var statistics = new SystemStatistics
            {
                TotalUsers = users.Count(),
                TotalQuestionBanks = banks.Count(),
                TotalQuestions = questions.Count(),
                TotalExamPapers = papers.Count(),
                TotalExams = examRecords.Count()
            };

            return statistics;
        }
    }
}
