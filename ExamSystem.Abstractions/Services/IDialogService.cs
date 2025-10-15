using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ExamSystem.Abstractions.Services
{
    /// <summary>
    /// 对话框服务接口
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// 显示消息对话框
        /// </summary>
        Task ShowMessageAsync(string title, string message);

        /// <summary>
        /// 显示确认对话框
        /// </summary>
        Task<bool> ShowConfirmAsync(string title, string message);

        /// <summary>
        /// 显示错误对话框
        /// </summary>
        Task ShowErrorAsync(string title, string error);

        /// <summary>
        /// 显示警告对话框
        /// </summary>
        Task ShowWarningAsync(string title, string warning);

        /// <summary>
        /// 显示输入对话框
        /// </summary>
        Task<string?> ShowInputDialogAsync(string title, string prompt, string defaultValue = "");

        /// <summary>
        /// 显示自定义对话框
        /// </summary>
        Task<bool?> ShowCustomDialogAsync(ObservableObject viewModel);
    }
}
