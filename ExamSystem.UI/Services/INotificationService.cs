namespace ExamSystem.UI.Services
{
    /// <summary>
    /// 通知服务接口
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// 显示成功提示
        /// </summary>
        void ShowSuccess(string message);

        /// <summary>
        /// 显示警告提示
        /// </summary>
        void ShowWarning(string message);

        /// <summary>
        /// 显示错误提示
        /// </summary>
        void ShowError(string message);

        /// <summary>
        /// 显示信息提示
        /// </summary>
        void ShowInfo(string message);
    }
}
