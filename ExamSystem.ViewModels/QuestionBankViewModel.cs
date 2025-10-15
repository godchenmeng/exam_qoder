using System.Collections.ObjectModel;
using System.Linq;
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
    /// 题库管理视图模型
    /// </summary>
    public class QuestionBankViewModel : ObservableObject
    {
        private readonly IQuestionService _questionService;
        private readonly IDialogService _dialogService;
        private readonly INotificationService _notificationService;

        private ObservableCollection<QuestionBank> _questionBanks = new();
        public ObservableCollection<QuestionBank> QuestionBanks
        {
            get => _questionBanks;
            set => SetProperty(ref _questionBanks, value);
        }

        private QuestionBank? _selectedBank;
        public QuestionBank? SelectedBank
        {
            get => _selectedBank;
            set
            {
                if (SetProperty(ref _selectedBank, value))
                {
                    _ = LoadQuestionsAsync();
                }
            }
        }

        private ObservableCollection<Question> _questions = new();
        public ObservableCollection<Question> Questions
        {
            get => _questions;
            set => SetProperty(ref _questions, value);
        }

        private Question? _selectedQuestion;
        public Question? SelectedQuestion
        {
            get => _selectedQuestion;
            set => SetProperty(ref _selectedQuestion, value);
        }

        private QuestionType? _questionTypeFilter;
        public QuestionType? QuestionTypeFilter
        {
            get => _questionTypeFilter;
            set
            {
                if (SetProperty(ref _questionTypeFilter, value))
                {
                    _ = LoadQuestionsAsync();
                }
            }
        }

        private Difficulty? _difficultyFilter;
        public Difficulty? DifficultyFilter
        {
            get => _difficultyFilter;
            set
            {
                if (SetProperty(ref _difficultyFilter, value))
                {
                    _ = LoadQuestionsAsync();
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public QuestionBankViewModel(
            IQuestionService questionService,
            IDialogService dialogService,
            INotificationService notificationService)
        {
            _questionService = questionService;
            _dialogService = dialogService;
            _notificationService = notificationService;

            _ = LoadQuestionBanksAsync();
        }

        private IAsyncRelayCommand? _loadQuestionBanksCommand;
        public IAsyncRelayCommand LoadQuestionBanksCommand => 
            _loadQuestionBanksCommand ??= new AsyncRelayCommand(LoadQuestionBanksAsync);

        private async Task LoadQuestionBanksAsync()
        {
            IsLoading = true;
            try
            {
                var banks = await _questionService.GetAllQuestionBanksAsync();
                QuestionBanks = new ObservableCollection<QuestionBank>(banks);
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载题库列表失败: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private IAsyncRelayCommand? _loadQuestionsCommand;
        public IAsyncRelayCommand LoadQuestionsCommand => 
            _loadQuestionsCommand ??= new AsyncRelayCommand(LoadQuestionsAsync);

        private async Task LoadQuestionsAsync()
        {
            if (SelectedBank == null)
            {
                Questions.Clear();
                return;
            }

            IsLoading = true;
            try
            {
                var questions = await _questionService.GetQuestionsByBankIdAsync(SelectedBank.Id);
                
                // 应用筛选
                var filtered = questions.AsEnumerable();
                if (QuestionTypeFilter.HasValue)
                {
                    filtered = filtered.Where(q => q.Type == QuestionTypeFilter.Value);
                }
                if (DifficultyFilter.HasValue)
                {
                    filtered = filtered.Where(q => q.Difficulty == DifficultyFilter.Value);
                }

                Questions = new ObservableCollection<Question>(filtered);
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载题目列表失败: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private IAsyncRelayCommand? _deleteQuestionCommand;
        public IAsyncRelayCommand DeleteQuestionCommand => 
            _deleteQuestionCommand ??= new AsyncRelayCommand(DeleteQuestionAsync, () => SelectedQuestion != null);

        private async Task DeleteQuestionAsync()
        {
            if (SelectedQuestion == null) return;

            var confirmed = await _dialogService.ShowConfirmAsync("确认删除", "确定要删除该题目吗？");
            if (!confirmed) return;

            try
            {
                await _questionService.DeleteQuestionAsync(SelectedQuestion.Id);
                _notificationService.ShowSuccess("题目删除成功");
                await LoadQuestionsAsync();
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"删除题目失败: {ex.Message}");
            }
        }
    }
}
