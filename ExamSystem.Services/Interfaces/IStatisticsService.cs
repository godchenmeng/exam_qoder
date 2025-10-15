using System.Collections.Generic;
using System.Threading.Tasks;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.DTOs;

namespace ExamSystem.Services.Interfaces
{
    /// <summary>
    /// 统计分析服务接口
    /// </summary>
    public interface IStatisticsService
    {
        /// <summary>
        /// 获取试卷统计
        /// </summary>
        Task<PaperStatistics> GetPaperStatisticsAsync(int paperId);

        /// <summary>
        /// 获取学生成绩统计
        /// </summary>
        Task<StudentScoreStatistics> GetStudentScoreStatisticsAsync(int userId);

        /// <summary>
        /// 获取题目分析
        /// </summary>
        Task<QuestionAnalysis> GetQuestionAnalysisAsync(int questionId, int paperId);

        /// <summary>
        /// 获取班级排名
        /// </summary>
        Task<List<StudentRanking>> GetClassRankingAsync(int paperId);

        /// <summary>
        /// 获取错题统计
        /// </summary>
        Task<List<WrongQuestion>> GetWrongQuestionsAsync(int userId);
    }

    /// <summary>
    /// 学生成绩统计
    /// </summary>
    public class StudentScoreStatistics
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int TotalExams { get; set; }
        public int PassedExams { get; set; }
        public decimal AverageScore { get; set; }
        public decimal HighestScore { get; set; }
        public decimal LowestScore { get; set; }
    }

    /// <summary>
    /// 学生排名
    /// </summary>
    public class StudentRanking
    {
        public int Rank { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string RealName { get; set; }
        public decimal TotalScore { get; set; }
        public bool IsPassed { get; set; }
    }

    /// <summary>
    /// 错题记录
    /// </summary>
    public class WrongQuestion
    {
        public int QuestionId { get; set; }
        public string QuestionContent { get; set; }
        public string QuestionType { get; set; }
        public string UserAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public int WrongCount { get; set; }
    }
}
