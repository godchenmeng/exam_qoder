using System;
using System.Linq;

namespace ExamSystem.Infrastructure.Utils
{
    /// <summary>
    /// 答案比较工具类
    /// </summary>
    public static class AnswerComparer
    {
        /// <summary>
        /// 精确比较答案
        /// </summary>
        /// <param name="userAnswer">用户答案</param>
        /// <param name="correctAnswer">正确答案</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <param name="trimWhitespace">是否去除空格</param>
        /// <returns>是否匹配</returns>
        public static bool ExactMatch(string userAnswer, string correctAnswer, 
            bool ignoreCase = true, bool trimWhitespace = true)
        {
            if (userAnswer == null || correctAnswer == null)
                return false;

            if (trimWhitespace)
            {
                userAnswer = userAnswer.Trim();
                correctAnswer = correctAnswer.Trim();
            }

            var comparison = ignoreCase 
                ? StringComparison.OrdinalIgnoreCase 
                : StringComparison.Ordinal;

            return string.Equals(userAnswer, correctAnswer, comparison);
        }

        /// <summary>
        /// 模糊比较答案(使用编辑距离算法)
        /// </summary>
        /// <param name="userAnswer">用户答案</param>
        /// <param name="correctAnswer">正确答案</param>
        /// <param name="threshold">相似度阈值(0-1)</param>
        /// <returns>是否匹配</returns>
        public static bool FuzzyMatch(string userAnswer, string correctAnswer, double threshold = 0.8)
        {
            if (string.IsNullOrEmpty(userAnswer) || string.IsNullOrEmpty(correctAnswer))
                return false;

            var similarity = CalculateSimilarity(userAnswer.ToLower(), correctAnswer.ToLower());
            return similarity >= threshold;
        }

        /// <summary>
        /// 计算两个字符串的相似度(Levenshtein距离)
        /// </summary>
        private static double CalculateSimilarity(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
                return 0;

            int maxLen = Math.Max(s1.Length, s2.Length);
            if (maxLen == 0)
                return 1.0;

            int distance = LevenshteinDistance(s1, s2);
            return 1.0 - (double)distance / maxLen;
        }

        /// <summary>
        /// 计算Levenshtein距离
        /// </summary>
        private static int LevenshteinDistance(string s1, string s2)
        {
            int[,] d = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
                d[i, 0] = i;

            for (int j = 0; j <= s2.Length; j++)
                d[0, j] = j;

            for (int i = 1; i <= s1.Length; i++)
            {
                for (int j = 1; j <= s2.Length; j++)
                {
                    int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[s1.Length, s2.Length];
        }

        /// <summary>
        /// 比较多选题答案
        /// </summary>
        /// <param name="userAnswer">用户答案(如"ABCD")</param>
        /// <param name="correctAnswer">正确答案(如"ABC")</param>
        /// <returns>匹配结果</returns>
        public static MultipleChoiceMatchResult CompareMultipleChoice(string userAnswer, string correctAnswer)
        {
            if (string.IsNullOrEmpty(userAnswer) || string.IsNullOrEmpty(correctAnswer))
                return new MultipleChoiceMatchResult { IsFullyCorrect = false };

            var userOptions = userAnswer.ToUpper().OrderBy(c => c).ToArray();
            var correctOptions = correctAnswer.ToUpper().OrderBy(c => c).ToArray();

            var userSet = new string(userOptions);
            var correctSet = new string(correctOptions);

            // 完全正确
            if (userSet == correctSet)
            {
                return new MultipleChoiceMatchResult
                {
                    IsFullyCorrect = true,
                    IsPartiallyCorrect = false,
                    HasWrongOption = false,
                    CorrectCount = correctOptions.Length,
                    TotalCorrectCount = correctOptions.Length
                };
            }

            // 检查是否有错误选项
            bool hasWrong = userOptions.Any(o => !correctOptions.Contains(o));

            if (hasWrong)
            {
                return new MultipleChoiceMatchResult
                {
                    IsFullyCorrect = false,
                    IsPartiallyCorrect = false,
                    HasWrongOption = true
                };
            }

            // 部分正确(选中的都对但未全选)
            int correctCount = userOptions.Count(o => correctOptions.Contains(o));

            return new MultipleChoiceMatchResult
            {
                IsFullyCorrect = false,
                IsPartiallyCorrect = true,
                HasWrongOption = false,
                CorrectCount = correctCount,
                TotalCorrectCount = correctOptions.Length
            };
        }
    }

    /// <summary>
    /// 多选题匹配结果
    /// </summary>
    public class MultipleChoiceMatchResult
    {
        /// <summary>
        /// 是否完全正确
        /// </summary>
        public bool IsFullyCorrect { get; set; }

        /// <summary>
        /// 是否部分正确
        /// </summary>
        public bool IsPartiallyCorrect { get; set; }

        /// <summary>
        /// 是否有错误选项
        /// </summary>
        public bool HasWrongOption { get; set; }

        /// <summary>
        /// 正确选项数量
        /// </summary>
        public int CorrectCount { get; set; }

        /// <summary>
        /// 总正确选项数量
        /// </summary>
        public int TotalCorrectCount { get; set; }
    }
}
