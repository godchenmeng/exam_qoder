using Microsoft.Win32;
using System.Threading.Tasks;
using ExamSystem.Abstractions.Services;

namespace ExamSystem.UI.Services
{
    /// <summary>
    /// 文件对话框服务实现
    /// </summary>
    public class FileDialogService : IFileDialogService
    {
        public Task<string[]?> OpenFileDialogAsync(string filter = "All Files (*.*)|*.*", bool multiSelect = false)
        {
            return Task.Run(() =>
            {
                var dialog = new OpenFileDialog
                {
                    Filter = filter,
                    Multiselect = multiSelect
                };

                return dialog.ShowDialog() == true ? dialog.FileNames : null;
            });
        }

        public Task<string?> SaveFileDialogAsync(string defaultFileName = "", string filter = "All Files (*.*)|*.*")
        {
            return Task.Run(() => ShowSaveFileDialog(filter, defaultFileName));
        }

        public Task<string?> OpenFolderDialogAsync()
        {
            return Task.Run(() => ShowFolderBrowserDialog());
        }

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
            return null;
            //using var dialog = new FolderBrowserDialog();
            //return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : null;
        }
    }
}
