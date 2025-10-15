using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamSystem.Domain.DTOs
{
    /// <summary>
    /// 考试统计信息
    /// </summary>
    public class ExamStatistics
    {
        /// <summary>
        /// 总考试数
        /// </summary>
        public int TotalExams { get; set; }

        /// <summary>
        /// 总学生数
        /// </summary>
        public int TotalStudents { get; set; }

        /// <summary>
        /// 平均分
        /// </summary>
        public double AverageScore { get; set; }

        /// <summary>
        /// 通过率
        /// </summary>
        public double PassingRate { get; set; }
    }
}