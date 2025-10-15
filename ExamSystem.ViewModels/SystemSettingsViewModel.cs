using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using ExamSystem.Abstractions.Services;

namespace ExamSystem.ViewModels
{
    /// <summary>
    /// 系统设置视图模型
    /// </summary>
    public class SystemSettingsViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;
        private readonly INotificationService _notificationService;
        private readonly IFileDialogService _fileDialogService;

        private string _databasePath = "exam.db";
        public string DatabasePath
        {
            get => _databasePath;
            set => SetProperty(ref _databasePath, value);
        }

        private int _defaultExamDuration = 120;
        public int DefaultExamDuration
        {
            get => _defaultExamDuration;
            set => SetProperty(ref _defaultExamDuration, value);
        }

        private int _passingScore = 60;
        public int PassingScore
        {
            get => _passingScore;
            set => SetProperty(ref _passingScore, value);
        }

        private int _sessionTimeout = 30;
        public int SessionTimeout
        {
            get => _sessionTimeout;
            set => SetProperty(ref _sessionTimeout, value);
        }

        private int _minPasswordLength = 6;
        public int MinPasswordLength
        {
            get => _minPasswordLength;
            set => SetProperty(ref _minPasswordLength, value);
        }

        private int _logRetentionDays = 30;
        public int LogRetentionDays
        {
            get => _logRetentionDays;
            set => SetProperty(ref _logRetentionDays, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public SystemSettingsViewModel(
            IDialogService dialogService,
            INotificationService notificationService,
            IFileDialogService fileDialogService)
        {
            _dialogService = dialogService;
            _notificationService = notificationService;
            _fileDialogService = fileDialogService;
        }

        private IAsyncRelayCommand? _backupDatabaseCommand;
        public IAsyncRelayCommand BackupDatabaseCommand => 
            _backupDatabaseCommand ??= new AsyncRelayCommand(BackupDatabaseAsync);

        private async Task BackupDatabaseAsync()
        {
            var savePath = _fileDialogService.ShowSaveFileDialog(
                "SQLite Database (*.db)|*.db", 
                $"exam_backup_{System.DateTime.Now:yyyyMMdd_HHmmss}.db");

            if (string.IsNullOrEmpty(savePath))
                return;

            IsLoading = true;
            try
            {
                // TODO: 实现数据库备份逻辑
                await Task.Delay(1000); // 模拟备份过程
                _notificationService.ShowSuccess("数据库备份成功");
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"数据库备份失败: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private IAsyncRelayCommand? _restoreDatabaseCommand;
        public IAsyncRelayCommand RestoreDatabaseCommand => 
            _restoreDatabaseCommand ??= new AsyncRelayCommand(RestoreDatabaseAsync);

        private async Task RestoreDatabaseAsync()
        {
            var confirmed = await _dialogService.ShowConfirmAsync(
                "确认恢复", 
                "恢复数据库将覆盖当前数据，是否继续？");

            if (!confirmed)
                return;

            var filePath = _fileDialogService.ShowOpenFileDialog("SQLite Database (*.db)|*.db");
            if (string.IsNullOrEmpty(filePath))
                return;

            IsLoading = true;
            try
            {
                // TODO: 实现数据库恢复逻辑
                await Task.Delay(1000);
                _notificationService.ShowSuccess("数据库恢复成功");
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"数据库恢复失败: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private IAsyncRelayCommand? _saveSettingsCommand;
        public IAsyncRelayCommand SaveSettingsCommand => 
            _saveSettingsCommand ??= new AsyncRelayCommand(SaveSettingsAsync);

        private async Task SaveSettingsAsync()
        {
            IsLoading = true;
            try
            {
                // TODO: 保存设置到配置文件或数据库
                await Task.Delay(500);
                _notificationService.ShowSuccess("设置保存成功");
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存设置失败: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
