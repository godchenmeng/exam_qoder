using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamSystem.Domain.DTOs
{
    /// <summary>
    /// 系统统计信息
    /// </summary>
    public class SystemStatistics
    {
        /// <summary>
        /// 总用户数
        /// </summary>
        public int TotalUsers { get; set; }

        /// <summary>
        /// 总题库数
        /// </summary>
        public int TotalQuestionBanks { get; set; }

        /// <summary>
        /// 总试卷数
        /// </summary>
        public int TotalExamPapers { get; set; }

        /// <summary>
        /// 总考试数
        /// </summary>
        public int TotalExams { get; set; }
    }
}