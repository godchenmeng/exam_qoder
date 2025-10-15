namespace ExamSystem.Infrastructure.Common
{
    /// <summary>
    /// 常量定义
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// 默认管理员用户名
        /// </summary>
        public const string DefaultAdminUsername = "admin";

        /// <summary>
        /// 默认管理员密码
        /// </summary>
        public const string DefaultAdminPassword = "admin123";

        /// <summary>
        /// 默认密码(用于批量导入)
        /// </summary>
        public const string DefaultPassword = "123456";

        /// <summary>
        /// 分数段定义
        /// </summary>
        public static class ScoreRanges
        {
            public const int ExcellentMin = 90;
            public const int GoodMin = 80;
            public const int MediumMin = 70;
            public const int PassMin = 60;
        }

        /// <summary>
        /// 难度系数
        /// </summary>
        public static class DifficultyCoefficients
        {
            public const double Easy = 1.0;
            public const double Medium = 1.5;
            public const double Hard = 2.0;
        }

        /// <summary>
        /// 文件格式
        /// </summary>
        public static class FileFormats
        {
            public const string Excel = ".xlsx";
            public const string Pdf = ".pdf";
            public const string Json = ".json";
        }

        /// <summary>
        /// 日期时间格式
        /// </summary>
        public static class DateTimeFormats
        {
            public const string Standard = "yyyy-MM-dd HH:mm:ss";
            public const string DateOnly = "yyyy-MM-dd";
            public const string TimeOnly = "HH:mm:ss";
        }
    }
}
