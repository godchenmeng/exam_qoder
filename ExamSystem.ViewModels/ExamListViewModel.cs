using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSystem.Domain.Entities;
using ExamSystem.Services.Interfaces;
using ExamSystem.Abstractions.Services;

namespace ExamSystem.ViewModels
{
    /// <summary>
    /// 考试列表视图模型
    /// </summary>
    public class ExamListViewModel : ObservableObject
    {
        private readonly IExamService _examService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        private ObservableCollection<ExamPaper> _upcomingExams = new();
        public ObservableCollection<ExamPaper> UpcomingExams
        {
            get => _upcomingExams;
            set => SetProperty(ref _upcomingExams, value);
        }

        private ObservableCollection<ExamRecord> _ongoingExams = new();
        public ObservableCollection<ExamRecord> OngoingExams
        {
            get => _ongoingExams;
            set => SetProperty(ref _ongoingExams, value);
        }

        private ObservableCollection<ExamRecord> _completedExams = new();
        public ObservableCollection<ExamRecord> CompletedExams
        {
            get => _completedExams;
            set => SetProperty(ref _completedExams, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ExamListViewModel(
            IExamService examService,
            INavigationService navigationService,
            IDialogService dialogService)
        {
            _examService = examService;
            _navigationService = navigationService;
            _dialogService = dialogService;

            _ = LoadExamsAsync();
        }

        private IAsyncRelayCommand? _loadExamsCommand;
        public IAsyncRelayCommand LoadExamsCommand => 
            _loadExamsCommand ??= new AsyncRelayCommand(LoadExamsAsync);

        private async Task LoadExamsAsync()
        {
            IsLoading = true;
            try
            {
                // TODO: 根据当前用户加载考试列表
                // var upcoming = await _examService.GetUpcomingExamsAsync(currentUserId);
                // var ongoing = await _examService.GetOngoingExamsAsync(currentUserId);
                // var completed = await _examService.GetCompletedExamsAsync(currentUserId);
                
                UpcomingExams = new ObservableCollection<ExamPaper>();
                OngoingExams = new ObservableCollection<ExamRecord>();
                CompletedExams = new ObservableCollection<ExamRecord>();
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载考试列表失败: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private IAsyncRelayCommand<ExamPaper>? _startExamCommand;
        public IAsyncRelayCommand<ExamPaper> StartExamCommand => 
            _startExamCommand ??= new AsyncRelayCommand<ExamPaper>(StartExamAsync);

        private async Task StartExamAsync(ExamPaper? examPaper)
        {
            if (examPaper == null) return;

            var confirmed = await _dialogService.ShowConfirmAsync(
                "开始考试", 
                $"确定要开始考试 {examPaper.Name} 吗？\n考试时长：{examPaper.Duration}分钟");

            if (!confirmed) return;

            try
            {
                var examRecord = await _examService.StartExamAsync(examPaper.PaperId, 0); // TODO: 传入当前用户ID
                // _navigationService.NavigateTo<ExamTakingViewModel>(examRecord);
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"开始考试失败: {ex.Message}");
            }
        }
    }
}
