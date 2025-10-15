using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Enums;
using ExamSystem.Domain.DTOs;
using ExamSystem.Repository.Interfaces;
using ExamSystem.Services.Interfaces;
using ExamSystem.Infrastructure.Utils;

namespace ExamSystem.Services.Implementations
{
    /// <summary>
    /// 试卷服务实现
    /// </summary>
    public class ExamPaperService : IExamPaperService
    {
        private readonly IExamPaperRepository _paperRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IExamRecordRepository _examRecordRepository;
        private readonly IRepository<PaperQuestion> _paperQuestionRepository;

        public ExamPaperService(
            IExamPaperRepository paperRepository,
            IQuestionRepository questionRepository,
            IExamRecordRepository examRecordRepository,
            IRepository<PaperQuestion> paperQuestionRepository)
        {
            _paperRepository = paperRepository ?? throw new ArgumentNullException(nameof(paperRepository));
            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
            _examRecordRepository = examRecordRepository ?? throw new ArgumentNullException(nameof(examRecordRepository));
            _paperQuestionRepository = paperQuestionRepository ?? throw new ArgumentNullException(nameof(paperQuestionRepository));
        }

        /// <summary>
        /// 创建固定试卷
        /// </summary>
        public async Task<ExamPaper> CreateFixedPaperAsync(ExamPaper paper, List<PaperQuestion> questions)
        {
            if (paper == null)
                throw new ArgumentNullException(nameof(paper));

            if (questions == null || !questions.Any())
                throw new ArgumentException("试卷至少需要包含一道题目", nameof(questions));

            // 验证试卷
            var validation = ValidatePaper(paper);
            if (!validation.IsValid)
                // 将所有 validation.ErrorMessage 替换为 string.Join(", ", validation.Errors)
                if (!validation.IsValid)
                    throw new InvalidOperationException($"试卷验证失败: {string.Join(", ", validation.Errors)}");

            // 设置试卷类型和状态
            paper.PaperType = PaperType.Fixed;
            paper.Status = PaperStatus.Draft;
            paper.CreatedAt = DateTime.Now;
            paper.TotalScore = questions.Sum(q => q.Score);

            // 保存试卷
            await _paperRepository.AddAsync(paper);
            await _paperRepository.SaveChangesAsync();

            // 保存试卷题目关联
            int orderIndex = 1;
            foreach (var question in questions)
            {
                question.PaperId = paper.PaperId;
                question.OrderIndex = orderIndex++;
                await _paperQuestionRepository.AddAsync(question);
            }
            await _paperQuestionRepository.SaveChangesAsync();

            return paper;
        }

        /// <summary>
        /// 创建随机试卷
        /// </summary>
        public async Task<ExamPaper> CreateRandomPaperAsync(ExamPaper paper, RandomPaperConfig config)
        {
            if (paper == null)
                throw new ArgumentNullException(nameof(paper));

            if (config == null || !config.Rules.Any())
                throw new ArgumentException("随机组卷配置不能为空", nameof(config));

            // 验证试卷
            var validation = ValidatePaper(paper);
            if (!validation.IsValid)
                throw new InvalidOperationException($"试卷验证失败: {string.Join(", ", validation.Errors)}");

            // 根据规则抽取题目
            var selectedQuestions = await SelectQuestionsAsync(config);

            // 设置试卷类型和状态
            paper.PaperType = PaperType.Random;
            paper.Status = PaperStatus.Draft;
            paper.CreatedAt = DateTime.Now;
            paper.TotalScore = selectedQuestions.Sum(q => q.Score);
            paper.RandomConfig = JsonHelper.Serialize(config);

            // 保存试卷
            await _paperRepository.AddAsync(paper);
            await _paperRepository.SaveChangesAsync();

            // 保存试卷题目关联
            foreach (var question in selectedQuestions)
            {
                question.PaperId = paper.PaperId;
                await _paperQuestionRepository.AddAsync(question);
            }
            await _paperQuestionRepository.SaveChangesAsync();

            return paper;
        }

        /// <summary>
        /// 创建混合试卷
        /// </summary>
        public async Task<ExamPaper> CreateMixedPaperAsync(ExamPaper paper, MixedPaperConfig config)
        {
            if (paper == null)
                throw new ArgumentNullException(nameof(paper));

            if (config == null)
                throw new ArgumentNullException(nameof(config));

            // 验证试卷
            var validation = ValidatePaper(paper);
            if (!validation.IsValid)
                throw new InvalidOperationException($"试卷验证失败: {string.Join(", ", validation.Errors)}");

            var allQuestions = new List<PaperQuestion>();
            int orderIndex = 1;

            // 添加固定题目
            if (config.FixedQuestionIds != null && config.FixedQuestionIds.Any())
            {
                foreach (var questionId in config.FixedQuestionIds)
                {
                    var question = await _questionRepository.GetByIdAsync(questionId);
                    if (question == null)
                        throw new InvalidOperationException($"题目ID {questionId} 不存在");

                    var score = config.FixedQuestionScores.ContainsKey(questionId) 
                        ? config.FixedQuestionScores[questionId] 
                        : question.DefaultScore;

                    allQuestions.Add(new PaperQuestion
                    {
                        QuestionId = questionId,
                        OrderIndex = orderIndex++,
                        Score = score
                    });
                }
            }

            // 添加随机题目
            if (config.RandomConfig != null && config.RandomConfig.Rules.Any())
            {
                var randomQuestions = await SelectQuestionsAsync(config.RandomConfig);
                foreach (var rq in randomQuestions)
                {
                    rq.OrderIndex = orderIndex++;
                    allQuestions.Add(rq);
                }
            }

            if (!allQuestions.Any())
                throw new ArgumentException("混合试卷至少需要包含一道题目");

            // 设置试卷类型和状态
            paper.PaperType = PaperType.Mixed;
            paper.Status = PaperStatus.Draft;
            paper.CreatedAt = DateTime.Now;
            paper.TotalScore = allQuestions.Sum(q => q.Score);
            paper.RandomConfig = JsonHelper.Serialize(config);

            // 保存试卷
            await _paperRepository.AddAsync(paper);
            await _paperRepository.SaveChangesAsync();

            // 保存试卷题目关联
            foreach (var question in allQuestions)
            {
                question.PaperId = paper.PaperId;
                await _paperQuestionRepository.AddAsync(question);
            }
            await _paperQuestionRepository.SaveChangesAsync();

            return paper;
        }

        /// <summary>
        /// 获取试卷详情
        /// </summary>
        public async Task<ExamPaper> GetPaperWithQuestionsAsync(int paperId)
        {
            return await _paperRepository.GetWithQuestionsAsync(paperId);
        }

        /// <summary>
        /// 获取试卷的题目列表
        /// </summary>
        public async Task<List<PaperQuestion>> GetPaperQuestionsAsync(int paperId)
        {
            var questions = await _paperQuestionRepository.FindAsync(pq => pq.PaperId == paperId);
            return questions.OrderBy(pq => pq.OrderIndex).ToList();
        }

        /// <summary>
        /// 更新试卷
        /// </summary>
        public async Task UpdatePaperAsync(ExamPaper paper)
        {
            if (paper == null)
                throw new ArgumentNullException(nameof(paper));

            var validation = ValidatePaper(paper);
            if (!validation.IsValid)
                throw new InvalidOperationException($"试卷验证失败: {string.Join(", ", validation.Errors)}");

            paper.UpdatedAt = DateTime.Now;
            await _paperRepository.UpdateAsync(paper);
            await _paperRepository.SaveChangesAsync();
        }

        /// <summary>
        /// 删除试卷
        /// </summary>
        public async Task DeletePaperAsync(int paperId)
        {
            // 检查是否有考试记录
            var records = await _examRecordRepository.GetByPaperIdAsync(paperId);
            if (records.Any())
                throw new InvalidOperationException("该试卷已有考试记录，无法删除");

            // 删除试卷题目关联
            var paperQuestions = await _paperQuestionRepository.FindAsync(pq => pq.PaperId == paperId);
            foreach (var pq in paperQuestions)
            {
                await _paperQuestionRepository.DeleteAsync(pq);
            }

            // 删除试卷
            await _paperRepository.DeleteAsync(paperId);
            await _paperRepository.SaveChangesAsync();
        }

        /// <summary>
        /// 激活试卷
        /// </summary>
        public async Task ActivatePaperAsync(int paperId)
        {
            var paper = await _paperRepository.GetByIdAsync(paperId);
            if (paper == null)
                throw new InvalidOperationException("试卷不存在");

            paper.Status = PaperStatus.Active;
            paper.UpdatedAt = DateTime.Now;
            await _paperRepository.UpdateAsync(paper);
            await _paperRepository.SaveChangesAsync();
        }

        /// <summary>
        /// 归档试卷
        /// </summary>
        public async Task ArchivePaperAsync(int paperId)
        {
            var paper = await _paperRepository.GetByIdAsync(paperId);
            if (paper == null)
                throw new InvalidOperationException("试卷不存在");

            paper.Status = PaperStatus.Archived;
            paper.UpdatedAt = DateTime.Now;
            await _paperRepository.UpdateAsync(paper);
            await _paperRepository.SaveChangesAsync();
        }

        /// <summary>
        /// 复制试卷
        /// </summary>
        public async Task<ExamPaper> DuplicatePaperAsync(int paperId)
        {
            var originalPaper = await _paperRepository.GetByIdAsync(paperId);
            if (originalPaper == null)
                throw new InvalidOperationException("试卷不存在");

            var originalQuestions = await GetPaperQuestionsAsync(paperId);

            // 创建新试卷
            var newPaper = new ExamPaper
            {
                Name = $"{originalPaper.Name} (副本)",
                Description = originalPaper.Description,
                TotalScore = originalPaper.TotalScore,
                PassScore = originalPaper.PassScore,
                Duration = originalPaper.Duration,
                PaperType = originalPaper.PaperType,
                RandomConfig = originalPaper.RandomConfig,
                CreatorId = originalPaper.CreatorId,
                Status = PaperStatus.Draft,
                CreatedAt = DateTime.Now
            };

            await _paperRepository.AddAsync(newPaper);
            await _paperRepository.SaveChangesAsync();

            // 复制题目关联
            foreach (var oq in originalQuestions)
            {
                var newPq = new PaperQuestion
                {
                    PaperId = newPaper.PaperId,
                    QuestionId = oq.QuestionId,
                    OrderIndex = oq.OrderIndex,
                    Score = oq.Score
                };
                await _paperQuestionRepository.AddAsync(newPq);
            }
            await _paperQuestionRepository.SaveChangesAsync();

            return newPaper;
        }

        /// <summary>
        /// 预览试卷
        /// </summary>
        public async Task<ExamPaper> PreviewPaperAsync(int paperId)
        {
            return await GetPaperWithQuestionsAsync(paperId);
        }

        /// <summary>
        /// 验证试卷
        /// </summary>
        public ValidationResult ValidatePaper(ExamPaper paper)
        {
            var result = new ValidationResult { IsValid = true, Errors = new List<string>() };

            if (string.IsNullOrWhiteSpace(paper.Name))
            {
                result.IsValid = false;
                result.AddError("试卷名称不能为空");
            }

            if (paper.Duration <= 0)
            {
                result.IsValid = false;
                result.AddError("考试时长必须大于0");
            }

            if (paper.PassScore < 0)
            {
                result.IsValid = false;
                result.AddError("及格分不能为负数");
            }

            if (paper.PassScore > paper.TotalScore && paper.TotalScore > 0)
            {
                result.IsValid = false;
                result.AddError("及格分不能大于总分");
            }

            return result;
        }

        /// <summary>
        /// 搜索试卷
        /// </summary>
        public async Task<PagedResult<ExamPaper>> SearchPapersAsync(string keyword, int? creatorId, int pageIndex, int pageSize)
        {
            return await _paperRepository.SearchPapersAsync(keyword, null, creatorId, pageIndex, pageSize);
        }

        /// <summary>
        /// 根据配置抽取题目（随机组卷算法）
        /// </summary>
        private async Task<List<PaperQuestion>> SelectQuestionsAsync(RandomPaperConfig config)
        {
            var selectedQuestions = new List<PaperQuestion>();
            int orderIndex = 1;
            var random = new Random();

            foreach (var rule in config.Rules)
            {
                // 查询符合条件的题目池
                var questionPool = await _questionRepository.GetQuestionsByBankAndTypeAsync(
                    config.BankId, 
                    rule.QuestionType, 
                    rule.Difficulty);

                var poolList = questionPool.ToList();

                // 检查题目数量是否充足
                if (poolList.Count < rule.Count)
                {
                    throw new InvalidOperationException(
                        $"题库中符合条件的题目不足：需要{rule.Count}道{rule.QuestionType}题，但只有{poolList.Count}道");
                }

                // Fisher-Yates洗牌算法随机抽取
                var selected = FisherYatesShuffle(poolList, random)
                    .Take(rule.Count)
                    .ToList();

                // 添加到试卷
                foreach (var question in selected)
                {
                    selectedQuestions.Add(new PaperQuestion
                    {
                        QuestionId = question.QuestionId,
                        OrderIndex = orderIndex++,
                        Score = rule.ScorePerQuestion
                    });
                }
            }

            return selectedQuestions;
        }

        /// <summary>
        /// Fisher-Yates洗牌算法
        /// </summary>
        private List<T> FisherYatesShuffle<T>(List<T> list, Random random)
        {
            var shuffled = new List<T>(list);
            int n = shuffled.Count;
            
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                var temp = shuffled[k];
                shuffled[k] = shuffled[n];
                shuffled[n] = temp;
            }

            return shuffled;
        }
    }
}
