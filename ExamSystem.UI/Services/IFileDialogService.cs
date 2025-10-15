namespace ExamSystem.UI.Services
{
    /// <summary>
    /// 文件对话框服务接口
    /// </summary>
    public interface IFileDialogService
    {
        /// <summary>
        /// 显示打开文件对话框
        /// </summary>
        string? ShowOpenFileDialog(string filter = "All Files (*.*)|*.*");

        /// <summary>
        /// 显示保存文件对话框
        /// </summary>
        string? ShowSaveFileDialog(string filter = "All Files (*.*)|*.*", string defaultFileName = "");

        /// <summary>
        /// 显示文件夹选择对话框
        /// </summary>
        string? ShowFolderBrowserDialog();
    }
}
