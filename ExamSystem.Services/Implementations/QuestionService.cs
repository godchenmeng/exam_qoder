using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.DTOs;
using ExamSystem.Domain.Enums;
using ExamSystem.Repository.Interfaces;
using ExamSystem.Services.Interfaces;

namespace ExamSystem.Services.Implementations
{
    /// <summary>
    /// 题目服务实现
    /// </summary>
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IRepository<Option> _optionRepository;

        public QuestionService(
            IQuestionRepository questionRepository,
            IRepository<Option> optionRepository)
        {
            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
            _optionRepository = optionRepository ?? throw new ArgumentNullException(nameof(optionRepository));
        }

        public async Task<Question> GetQuestionByIdAsync(int questionId)
        {
            return await _questionRepository.GetByIdAsync(questionId);
        }

        public async Task<Question> CreateQuestionAsync(Question question, List<Option> options = null)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));

            // 验证题目
            var validation = ValidateQuestion(question, options);
            if (!validation.IsValid)
                throw new InvalidOperationException(string.Join(", ", validation.Errors));

            question.CreatedAt = DateTime.Now;

            await _questionRepository.AddAsync(question);
            await _questionRepository.SaveChangesAsync();

            // 如果是选择题,保存选项
            if (options != null && options.Any() &&
                (question.QuestionType == QuestionType.SingleChoice ||
                 question.QuestionType == QuestionType.MultipleChoice))
            {
                foreach (var option in options)
                {
                    option.QuestionId = question.QuestionId;
                    await _optionRepository.AddAsync(option);
                }
                await _optionRepository.SaveChangesAsync();
            }

            return question;
        }

        public async Task<bool> UpdateQuestionAsync(Question question, List<Option> options = null)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));

            var validation = ValidateQuestion(question, options);
            if (!validation.IsValid)
                throw new InvalidOperationException(string.Join(", ", validation.Errors));

            question.UpdatedAt = DateTime.Now;

            await _questionRepository.UpdateAsync(question);
            var result = await _questionRepository.SaveChangesAsync();

            // 更新选项(先删除旧选项,再添加新选项)
            if (options != null &&
                (question.QuestionType == QuestionType.SingleChoice ||
                 question.QuestionType == QuestionType.MultipleChoice))
            {
                // 删除旧选项
                var oldOptions = await _optionRepository.FindAsync(o => o.QuestionId == question.QuestionId);
                foreach (var oldOption in oldOptions)
                {
                    await _optionRepository.DeleteAsync(oldOption);
                }

                // 添加新选项
                foreach (var option in options)
                {
                    option.QuestionId = question.QuestionId;
                    await _optionRepository.AddAsync(option);
                }

                await _optionRepository.SaveChangesAsync();
            }

            return result > 0;
        }

        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
                return false;

            await _questionRepository.DeleteAsync(question);
            var result = await _questionRepository.SaveChangesAsync();
            return result > 0;
        }

        public async Task<PagedResult<Question>> GetQuestionsByBankAsync(int bankId, int pageIndex, int pageSize)
        {
            return await _questionRepository.SearchQuestionsAsync(
                bankId, null, null, null, pageIndex, pageSize);
        }

        public async Task<PagedResult<Question>> SearchQuestionsAsync(
            int? bankId,
            QuestionType? questionType,
            Difficulty? difficulty,
            string keyword,
            int pageIndex,
            int pageSize)
        {
            return await _questionRepository.SearchQuestionsAsync(
                bankId, questionType, difficulty, keyword, pageIndex, pageSize);
        }

        public ValidationResult ValidateQuestion(Question question, List<Option> options = null)
        {
            var result = new ValidationResult { IsValid = true };

            if (question == null)
            {
                result.AddError("题目不能为空");
                return result;
            }

            if (string.IsNullOrWhiteSpace(question.Content))
                result.AddError("题目内容不能为空");

            if (question.Content?.Length > 5000)
                result.AddError("题目内容不能超过5000字符");

            if (string.IsNullOrWhiteSpace(question.Answer))
                result.AddError("答案不能为空");

            if (question.DefaultScore <= 0)
                result.AddError("分值必须大于0");

            if (question.DefaultScore > 100)
                result.AddError("分值不能超过100");

            // 选择题选项验证
            if (question.QuestionType == QuestionType.SingleChoice ||
                question.QuestionType == QuestionType.MultipleChoice)
            {
                if (options == null || !options.Any())
                {
                    result.AddError("选择题必须提供选项");
                }
                else
                {
                    if (options.Count < 2)
                        result.AddError("选项数量不能少于2个");

                    if (options.Count > 10)
                        result.AddError("选项数量不能超过10个");

                    var correctOptions = options.Where(o => o.IsCorrect).ToList();

                    if (question.QuestionType == QuestionType.SingleChoice)
                    {
                        if (correctOptions.Count != 1)
                            result.AddError("单选题必须有且仅有一个正确答案");
                    }
                    else if (question.QuestionType == QuestionType.MultipleChoice)
                    {
                        if (correctOptions.Count < 2)
                            result.AddError("多选题至少需要2个正确答案");
                    }
                }
            }

            return result;
        }

        public async Task<ImportResult> ImportQuestionsAsync(int bankId, IEnumerable<Question> questions)
        {
            var result = new ImportResult();

            foreach (var question in questions)
            {
                try
                {
                    question.BankId = bankId;
                    var validation = ValidateQuestion(question);

                    if (!validation.IsValid)
                    {
                        result.FailureCount++;
                        result.Errors.Add($"题目验证失败: {string.Join(", ", validation.Errors)}");
                        continue;
                    }

                    question.CreatedAt = DateTime.Now;
                    await _questionRepository.AddAsync(question);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.FailureCount++;
                    result.Errors.Add($"导入题目失败: {ex.Message}");
                }
            }

            if (result.SuccessCount > 0)
            {
                await _questionRepository.SaveChangesAsync();
            }

            return result;
        }

        public async Task<byte[]> ExportQuestionsAsync(IEnumerable<int> questionIds, string format)
        {
            // 导出功能需要Excel库支持,这里返回空实现
            // 实际应用中可以使用EPPlus或NPOI库
            await Task.CompletedTask;
            return Array.Empty<byte>();
        }

        public async Task<Question> DuplicateQuestionAsync(int questionId)
        {
            var original = await _questionRepository.GetWithOptionsAsync(questionId);
            if (original == null)
                throw new InvalidOperationException("题目不存在");

            var duplicate = new Question
            {
                BankId = original.BankId,
                QuestionType = original.QuestionType,
                Content = original.Content + " (副本)",
                Answer = original.Answer,
                Analysis = original.Analysis,
                DefaultScore = original.DefaultScore,
                Difficulty = original.Difficulty,
                Tags = original.Tags,
                CreatedAt = DateTime.Now
            };

            await _questionRepository.AddAsync(duplicate);
            await _questionRepository.SaveChangesAsync();

            return duplicate;
        }

        public async Task<Question> GetQuestionWithOptionsAsync(int questionId)
        {
            return await _questionRepository.GetWithOptionsAsync(questionId);
        }
    }
}
