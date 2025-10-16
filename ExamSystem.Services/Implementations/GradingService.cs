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
    /// 评分服务实现
    /// </summary>
    public class GradingService : IGradingService
    {
        private readonly IExamRecordRepository _examRecordRepository;
        private readonly IAnswerRecordRepository _answerRecordRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IRepository<PaperQuestion> _paperQuestionRepository;

        public GradingService(
            IExamRecordRepository examRecordRepository,
            IAnswerRecordRepository answerRecordRepository,
            IQuestionRepository questionRepository,
            IRepository<PaperQuestion> paperQuestionRepository)
        {
            _examRecordRepository = examRecordRepository ?? throw new ArgumentNullException(nameof(examRecordRepository));
            _answerRecordRepository = answerRecordRepository ?? throw new ArgumentNullException(nameof(answerRecordRepository));
            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
            _paperQuestionRepository = paperQuestionRepository ?? throw new ArgumentNullException(nameof(paperQuestionRepository));
        }

        /// <summary>
        /// 自动评分客观题
        /// </summary>
        public async Task AutoGradeObjectiveQuestionsAsync(int recordId)
        {
            var answerRecords = await _answerRecordRepository.GetByExamRecordAsync(recordId);
            double objectiveScore = 0;

            foreach (var answer in answerRecords)
            {
                var question = await _questionRepository.GetByIdAsync(answer.QuestionId);
                if (question == null)
                    continue;

                // 只评分客观题
                if (question.QuestionType == QuestionType.ShortAnswer || 
                    question.QuestionType == QuestionType.Essay)
                    continue;

                var paperQuestion = await _paperQuestionRepository
                    .FindAsync(pq => pq.PaperId == answer.ExamRecord.PaperId && pq.QuestionId == answer.QuestionId);
                var score = paperQuestion.FirstOrDefault()?.Score ?? question.DefaultScore;

                switch (question.QuestionType)
                {
                    case QuestionType.SingleChoice:
                        GradeSingleChoice(answer, question, score);
                        break;

                    case QuestionType.MultipleChoice:
                        GradeMultipleChoice(answer, question, score);
                        break;

                    case QuestionType.TrueFalse:
                        GradeTrueFalse(answer, question, score);
                        break;

                    case QuestionType.FillBlank:
                        GradeFillBlank(answer, question, score);
                        break;
                }

                answer.IsGraded = true;
                await _answerRecordRepository.UpdateAsync(answer);

                if (answer.Score > 0)
                    objectiveScore += answer.Score;
            }

            await _answerRecordRepository.SaveChangesAsync();

            // 更新考试记录的客观题得分
            var examRecord = await _examRecordRepository.GetByIdAsync(recordId);
            examRecord.ObjectiveScore = objectiveScore;
            await _examRecordRepository.UpdateAsync(examRecord);
            await _examRecordRepository.SaveChangesAsync();
        }

        /// <summary>
        /// 人工评分主观题
        /// </summary>
        public async Task ManualGradeSubjectiveQuestionAsync(int answerRecordId, double score, string comment, int graderId)
        {
            var answer = await _answerRecordRepository.GetByIdAsync(answerRecordId);
            if (answer == null)
                throw new InvalidOperationException("答题记录不存在");

            var question = await _questionRepository.GetByIdAsync(answer.QuestionId);
            if (question.QuestionType != QuestionType.ShortAnswer && 
                question.QuestionType != QuestionType.Essay)
                throw new InvalidOperationException("只能人工评分主观题");

            var paperQuestion = await _paperQuestionRepository
                .FindAsync(pq => pq.PaperId == answer.ExamRecord.PaperId && pq.QuestionId == answer.QuestionId);
            var maxScore = paperQuestion.FirstOrDefault()?.Score ?? question.DefaultScore;

            if (score < 0 || score > maxScore)
                throw new ArgumentException($"分数必须在0到{maxScore}之间");

            answer.Score = score;
            answer.IsCorrect = score == maxScore;
            answer.IsGraded = true;
            answer.GraderId = graderId;
            answer.GradeComment = comment;

            await _answerRecordRepository.UpdateAsync(answer);
            await _answerRecordRepository.SaveChangesAsync();

            // 检查是否所有主观题都已评分
            await CheckAndCalculateTotalScoreAsync(answer.RecordId);
        }

        /// <summary>
        /// 批量评分
        /// </summary>
        public async Task BatchGradeSubjectiveQuestionsAsync(List<GradeItem> gradeItems, int graderId)
        {
            foreach (var item in gradeItems)
            {
                await ManualGradeSubjectiveQuestionAsync(
                    item.AnswerRecordId, 
                    item.Score, 
                    item.Comment, 
                    graderId);
            }
        }

        /// <summary>
        /// 重新评分
        /// </summary>
        public async Task RegradePaperAsync(int recordId)
        {
            // 重置所有答题记录的评分状态
            var answerRecords = await _answerRecordRepository.GetByExamRecordAsync(recordId);
            foreach (var answer in answerRecords)
            {
                answer.Score = 0;
                answer.IsCorrect = false;
                answer.IsGraded = false;
                await _answerRecordRepository.UpdateAsync(answer);
            }
            await _answerRecordRepository.SaveChangesAsync();

            // 重新自动评分客观题
            await AutoGradeObjectiveQuestionsAsync(recordId);

            // 重置考试记录状态
            var examRecord = await _examRecordRepository.GetByIdAsync(recordId);
            examRecord.SubjectiveScore = 0;
            examRecord.TotalScore = 0;
            examRecord.IsPassed = null;
            await _examRecordRepository.UpdateAsync(examRecord);
            await _examRecordRepository.SaveChangesAsync();
        }

        /// <summary>
        /// 获取待评分列表
        /// </summary>
        public async Task<IEnumerable<ExamRecord>> GetPendingGradingRecordsAsync()
        {
            return await _examRecordRepository.GetRecordsNeedGradingAsync();
        }

        /// <summary>
        /// 计算总分
        /// </summary>
        public async Task CalculateTotalScoreAsync(int recordId)
        {
            var answerRecords = await _answerRecordRepository.GetByExamRecordAsync(recordId);
            var examRecord = await _examRecordRepository.GetWithDetailsAsync(recordId);

            // 计算客观题得分
            var objectiveScore = answerRecords
                .Where(a => a.Question.QuestionType != QuestionType.ShortAnswer 
                    && a.Question.QuestionType != QuestionType.Essay 
                    && a.IsGraded)
                .Sum(a => a.Score);

            // 计算主观题得分
            var subjectiveScore = answerRecords
                .Where(a => (a.Question.QuestionType == QuestionType.ShortAnswer 
                    || a.Question.QuestionType == QuestionType.Essay) 
                    && a.IsGraded)
                .Sum(a => a.Score);

            // 更新考试记录
            examRecord.ObjectiveScore = objectiveScore;
            examRecord.SubjectiveScore = subjectiveScore;
            examRecord.TotalScore = objectiveScore + subjectiveScore;
            examRecord.IsPassed = examRecord.TotalScore >= examRecord.ExamPaper.PassScore;
            examRecord.Status = ExamStatus.Graded;

            await _examRecordRepository.UpdateAsync(examRecord);
            await _examRecordRepository.SaveChangesAsync();
        }

        /// <summary>
        /// 单选题评分
        /// </summary>
        private void GradeSingleChoice(AnswerRecord answer, Question question, double score)
        {
            var isCorrect = AnswerComparer.ExactMatch(answer.UserAnswer, question.Answer);
            answer.Score = isCorrect ? score : 0;
            answer.IsCorrect = isCorrect;
        }

        /// <summary>
        /// 多选题评分
        /// </summary>
        private void GradeMultipleChoice(AnswerRecord answer, Question question, double score)
        {
            var result = AnswerComparer.CompareMultipleChoice(answer.UserAnswer, question.Answer);

            if (result.IsFullyCorrect)
            {
                // 完全正确
                answer.Score = score;
                answer.IsCorrect = true;
            }
            else if (result.IsPartiallyCorrect)
            {
                // 部分正确
                answer.Score = score * 0.5;
                answer.IsCorrect = false;
            }
            else
            {
                // 错误
                answer.Score = 0;
                answer.IsCorrect = false;
            }
        }

        /// <summary>
        /// 判断题评分
        /// </summary>
        private void GradeTrueFalse(AnswerRecord answer, Question question, double score)
        {
            var isCorrect = AnswerComparer.ExactMatch(answer.UserAnswer, question.Answer);
            answer.Score = isCorrect ? score : 0;
            answer.IsCorrect = isCorrect;
        }

        /// <summary>
        /// 填空题评分
        /// </summary>
        private void GradeFillBlank(AnswerRecord answer, Question question, double score)
        {
            // 使用模糊匹配
            var isCorrect = AnswerComparer.FuzzyMatch(answer.UserAnswer, question.Answer);
            answer.Score = isCorrect ? score : 0;
            answer.IsCorrect = isCorrect;
        }

        /// <summary>
        /// 检查并计算总分
        /// </summary>
        private async Task CheckAndCalculateTotalScoreAsync(int recordId)
        {
            var subjectiveAnswers = await _answerRecordRepository.GetSubjectiveAnswersAsync(recordId);
            
            // 如果所有主观题都已评分，则计算总分
            if (subjectiveAnswers.All(a => a.IsGraded))
            {
                await CalculateTotalScoreAsync(recordId);
            }
        }
    }
}
