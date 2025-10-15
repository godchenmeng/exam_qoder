using Microsoft.Win32;
using System.Windows.Forms;

namespace ExamSystem.UI.Services
{
    /// <summary>
    /// 文件对话框服务实现
    /// </summary>
    public class FileDialogService : IFileDialogService
    {
        public string? ShowOpenFileDialog(string filter = "All Files (*.*)|*.*")
        {
            var dialog = new OpenFileDialog
            {
                Filter = filter,
                Multiselect = false
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public string? ShowSaveFileDialog(string filter = "All Files (*.*)|*.*", string defaultFileName = "")
        {
            var dialog = new SaveFileDialog
            {
                Filter = filter,
                FileName = defaultFileName
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        public string? ShowFolderBrowserDialog()
        {
            using var dialog = new FolderBrowserDialog();
            return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : null;
        }
    }
}
