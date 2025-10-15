namespace ExamSystem.Infrastructure.Common
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class SystemConfig
    {
        /// <summary>
        /// 默认考试时长(分钟)
        /// </summary>
        public int DefaultExamDuration { get; set; } = 90;

        /// <summary>
        /// 答案自动保存间隔(秒)
        /// </summary>
        public int AutoSaveInterval { get; set; } = 30;

        /// <summary>
        /// 最大登录失败次数
        /// </summary>
        public int MaxLoginAttempts { get; set; } = 5;

        /// <summary>
        /// 会话超时时间(分钟)
        /// </summary>
        public int SessionTimeout { get; set; } = 120;

        /// <summary>
        /// 密码最小长度
        /// </summary>
        public int PasswordMinLength { get; set; } = 6;

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 启用窗口监控
        /// </summary>
        public bool EnableWindowMonitor { get; set; } = true;

        /// <summary>
        /// 时间警告提前量(分钟)
        /// </summary>
        public int TimeWarningMinutes { get; set; } = 5;

        /// <summary>
        /// 自动备份间隔(天)
        /// </summary>
        public int BackupInterval { get; set; } = 7;

        /// <summary>
        /// 日志保留天数
        /// </summary>
        public int LogRetentionDays { get; set; } = 30;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string DatabaseConnectionString { get; set; } = "Data Source=ExamSystem.db";
    }
}
