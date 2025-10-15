using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Enums;
using ExamSystem.Services.Interfaces;
using ExamSystem.Abstractions.Services;

namespace ExamSystem.ViewModels
{
    /// <summary>
    /// 试卷管理视图模型
    /// </summary>
    public class ExamPaperViewModel : ObservableObject
    {
        private readonly IExamPaperService _examPaperService;
        private readonly IDialogService _dialogService;
        private readonly INotificationService _notificationService;

        private ObservableCollection<ExamPaper> _examPapers = new();
        public ObservableCollection<ExamPaper> ExamPapers
        {
            get => _examPapers;
            set => SetProperty(ref _examPapers, value);
        }

        private ExamPaper? _selectedPaper;
        public ExamPaper? SelectedPaper
        {
            get => _selectedPaper;
            set => SetProperty(ref _selectedPaper, value);
        }

        private PaperStatus? _statusFilter;
        public PaperStatus? StatusFilter
        {
            get => _statusFilter;
            set
            {
                if (SetProperty(ref _statusFilter, value))
                {
                    _ = LoadExamPapersAsync();
                }
            }
        }

        private PaperType? _typeFilter;
        public PaperType? TypeFilter
        {
            get => _typeFilter;
            set
            {
                if (SetProperty(ref _typeFilter, value))
                {
                    _ = LoadExamPapersAsync();
                }
            }
        }

        private string _searchKeyword = string.Empty;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                if (SetProperty(ref _searchKeyword, value))
                {
                    _ = LoadExamPapersAsync();
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ExamPaperViewModel(
            IExamPaperService examPaperService,
            IDialogService dialogService,
            INotificationService notificationService)
        {
            _examPaperService = examPaperService;
            _dialogService = dialogService;
            _notificationService = notificationService;

            _ = LoadExamPapersAsync();
        }

        private IAsyncRelayCommand? _loadExamPapersCommand;
        public IAsyncRelayCommand LoadExamPapersCommand => 
            _loadExamPapersCommand ??= new AsyncRelayCommand(LoadExamPapersAsync);

        private async Task LoadExamPapersAsync()
        {
            IsLoading = true;
            try
            {
                var papers = await _examPaperService.GetExamPapersAsync(StatusFilter, TypeFilter, SearchKeyword);
                ExamPapers = new ObservableCollection<ExamPaper>(papers);
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载试卷列表失败: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private IAsyncRelayCommand? _deletePaperCommand;
        public IAsyncRelayCommand DeletePaperCommand => 
            _deletePaperCommand ??= new AsyncRelayCommand(DeletePaperAsync, () => SelectedPaper != null);

        private async Task DeletePaperAsync()
        {
            if (SelectedPaper == null) return;

            var confirmed = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除试卷 {SelectedPaper.Name} 吗？");
            if (!confirmed) return;

            try
            {
                await _examPaperService.DeleteExamPaperAsync(SelectedPaper.PaperId);
                _notificationService.ShowSuccess("试卷删除成功");
                await LoadExamPapersAsync();
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"删除试卷失败: {ex.Message}");
            }
        }
    }
}
