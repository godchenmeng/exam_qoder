using System.Threading.Tasks;

namespace ExamSystem.Abstractions.Services
{
    /// <summary>
    /// 文件对话框服务接口
    /// </summary>
    public interface IFileDialogService
    {
        /// <summary>
        /// 打开文件对话框（异步）
        /// </summary>
        /// <param name="filter">文件过滤器</param>
        /// <param name="multiSelect">是否支持多选</param>
        /// <returns>选中的文件路径数组</returns>
        Task<string[]?> OpenFileDialogAsync(string filter = "All Files (*.*)|*.*", bool multiSelect = false);

        /// <summary>
        /// 保存文件对话框（异步）
        /// </summary>
        /// <param name="defaultFileName">默认文件名</param>
        /// <param name="filter">文件过滤器</param>
        /// <returns>保存文件路径</returns>
        Task<string?> SaveFileDialogAsync(string defaultFileName = "", string filter = "All Files (*.*)|*.*");

        /// <summary>
        /// 文件夹选择对话框（异步）
        /// </summary>
        /// <returns>选中的文件夹路径</returns>
        Task<string?> OpenFolderDialogAsync();

        /// <summary>
        /// 显示打开文件对话框（同步版本，保持向后兼容）
        /// </summary>
        string? ShowOpenFileDialog(string filter = "All Files (*.*)|*.*");

        /// <summary>
        /// 显示保存文件对话框（同步版本，保持向后兼容）
        /// </summary>
        string? ShowSaveFileDialog(string filter = "All Files (*.*)|*.*", string defaultFileName = "");

        /// <summary>
        /// 显示文件夹选择对话框（同步版本，保持向后兼容）
        /// </summary>
        string? ShowFolderBrowserDialog();
    }
}
