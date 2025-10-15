using System;
using System.Windows;

namespace ExamSystem.UI.Services
{
    /// <summary>
    /// 通知服务实现
    /// </summary>
    public class NotificationService : INotificationService
    {
        public void ShowSuccess(string message)
        {
            ShowNotification(message, "成功", MessageBoxImage.Information);
        }

        public void ShowWarning(string message)
        {
            ShowNotification(message, "警告", MessageBoxImage.Warning);
        }

        public void ShowError(string message)
        {
            ShowNotification(message, "错误", MessageBoxImage.Error);
        }

        public void ShowInfo(string message)
        {
            ShowNotification(message, "提示", MessageBoxImage.Information);
        }

        private void ShowNotification(string message, string title, MessageBoxImage icon)
        {
            Application.Current?.Dispatcher.InvokeAsync(() =>
            {
                // 简单实现使用MessageBox
                // 实际项目中可以使用Snackbar或Toast通知
                MessageBox.Show(message, title, MessageBoxButton.OK, icon);
            });
        }
    }
}
