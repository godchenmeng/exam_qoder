using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using ExamSystem.Abstractions.Services;

namespace ExamSystem.UI.Services
{
    /// <summary>
    /// 对话框服务实现
    /// </summary>
    public class DialogService : IDialogService
    {
        public Task ShowMessageAsync(string title, string message)
        {
            return Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
            }).Task;
        }

        public Task<bool> ShowConfirmAsync(string title, string message)
        {
            return Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                return result == MessageBoxResult.Yes;
            }).Task;
        }

        public Task ShowErrorAsync(string title, string error)
        {
            return Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show(error, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }).Task;
        }

        public Task ShowWarningAsync(string title, string warning)
        {
            return Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show(warning, title, MessageBoxButton.OK, MessageBoxImage.Warning);
            }).Task;
        }

        public Task<string?> ShowInputDialogAsync(string title, string prompt, string defaultValue = "")
        {
            // 简单实现，实际项目中可以创建自定义输入对话框
            return Task.FromResult<string?>(defaultValue);
        }

        public Task<bool?> ShowCustomDialogAsync(ObservableObject viewModel)
        {
            // 需要配合自定义对话框窗口实现
            return Task.FromResult<bool?>(null);
        }
    }
}
