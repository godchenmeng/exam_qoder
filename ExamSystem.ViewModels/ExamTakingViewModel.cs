using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSystem.Domain.Entities;
using ExamSystem.Services.Interfaces;
using ExamSystem.Abstractions.Services;

namespace ExamSystem.ViewModels
{
    /// <summary>
    /// 在线答题视图模型
    /// </summary>
    public class ExamTakingViewModel : ObservableObject
    {
        private readonly IExamService _examService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private DispatcherTimer? _autoSaveTimer;
        private DispatcherTimer? _countdownTimer;

        private ExamRecord? _examRecord;
        public ExamRecord? ExamRecord
        {
            get => _examRecord;
            set => SetProperty(ref _examRecord, value);
        }

        private ObservableCollection<Question> _questions = new();
        public ObservableCollection<Question> Questions
        {
            get => _questions;
            set => SetProperty(ref _questions, value);
        }

        private int _currentQuestionIndex;
        public int CurrentQuestionIndex
        {
            get => _currentQuestionIndex;
            set
            {
                if (SetProperty(ref _currentQuestionIndex, value))
                {
                    OnPropertyChanged(nameof(CurrentQuestion));
                }
            }
        }

        public Question? CurrentQuestion => 
            Questions.Count > 0 && CurrentQuestionIndex >= 0 && CurrentQuestionIndex < Questions.Count 
                ? Questions[CurrentQuestionIndex] 
                : null;

        private int _remainingSeconds;
        public int RemainingSeconds
        {
            get => _remainingSeconds;
            set
            {
                if (SetProperty(ref _remainingSeconds, value))
                {
                    OnPropertyChanged(nameof(RemainingTime));
                }
            }
        }

        public string RemainingTime
        {
            get
            {
                var timeSpan = TimeSpan.FromSeconds(RemainingSeconds);
                return $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            }
        }

        private Dictionary<int, string> _answers = new();
        public Dictionary<int, string> Answers
        {
            get => _answers;
            set => SetProperty(ref _answers, value);
        }

        private HashSet<int> _markedQuestions = new();
        public HashSet<int> MarkedQuestions
        {
            get => _markedQuestions;
            set => SetProperty(ref _markedQuestions, value);
        }

        public ExamTakingViewModel(
            IExamService examService,
            IDialogService dialogService,
            INavigationService navigationService)
        {
            _examService = examService;
            _dialogService = dialogService;
            _navigationService = navigationService;
        }

        public void Initialize(ExamRecord examRecord)
        {
            ExamRecord = examRecord;
            _ = LoadQuestionsAsync();
            StartTimers();
        }

        private async Task LoadQuestionsAsync()
        {
            try
            {
                if (ExamRecord?.ExamPaper == null) return;

                var questions = await _examService.GetExamQuestionsAsync(ExamRecord.ExamPaperId);
                Questions = new ObservableCollection<Question>(questions);
                
                // 加载已保存的答案
                var savedAnswers = await _examService.GetSavedAnswersAsync(ExamRecord.Id);
                Answers = savedAnswers.ToDictionary(a => a.QuestionId, a => a.Answer);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载题目失败: {ex.Message}");
            }
        }

        private void StartTimers()
        {
            // 自动保存定时器（每分钟）
            _autoSaveTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            _autoSaveTimer.Tick += async (s, e) => await SaveAnswersAsync();
            _autoSaveTimer.Start();

            // 倒计时定时器
            _countdownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _countdownTimer.Tick += CountdownTimer_Tick;
            _countdownTimer.Start();
        }

        private void CountdownTimer_Tick(object? sender, EventArgs e)
        {
            if (RemainingSeconds > 0)
            {
                RemainingSeconds--;
            }
            else
            {
                _countdownTimer?.Stop();
                _ = SubmitExamAsync();
            }
        }

        private IRelayCommand? _nextQuestionCommand;
        public IRelayCommand NextQuestionCommand => 
            _nextQuestionCommand ??= new RelayCommand(NextQuestion, () => CurrentQuestionIndex < Questions.Count - 1);

        private void NextQuestion()
        {
            if (CurrentQuestionIndex < Questions.Count - 1)
            {
                CurrentQuestionIndex++;
            }
        }

        private IRelayCommand? _previousQuestionCommand;
        public IRelayCommand PreviousQuestionCommand => 
            _previousQuestionCommand ??= new RelayCommand(PreviousQuestion, () => CurrentQuestionIndex > 0);

        private void PreviousQuestion()
        {
            if (CurrentQuestionIndex > 0)
            {
                CurrentQuestionIndex--;
            }
        }

        private IAsyncRelayCommand? _submitExamCommand;
        public IAsyncRelayCommand SubmitExamCommand => 
            _submitExamCommand ??= new AsyncRelayCommand(SubmitExamAsync);

        private async Task SubmitExamAsync()
        {
            var confirmed = await _dialogService.ShowConfirmAsync("提交试卷", "确定要提交试卷吗？提交后将无法修改答案。");
            if (!confirmed) return;

            try
            {
                _autoSaveTimer?.Stop();
                _countdownTimer?.Stop();

                await _examService.SubmitExamAsync(ExamRecord!.Id);
                await _dialogService.ShowMessageAsync("提交成功", "试卷已提交，等待评分。");
                _navigationService.GoBack();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"提交试卷失败: {ex.Message}");
            }
        }

        private async Task SaveAnswersAsync()
        {
            try
            {
                // TODO: 保存答案到数据库
                // await _examService.SaveAnswersAsync(ExamRecord.Id, Answers);
            }
            catch
            {
                // 静默失败，不影响用户答题
            }
        }
    }
}
