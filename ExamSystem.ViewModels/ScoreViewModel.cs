using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using ExamSystem.Domain.Entities;
using ExamSystem.Services.Interfaces;
using ExamSystem.UI.Services;

namespace ExamSystem.ViewModels
{
    /// <summary>
    /// 成绩查看视图模型
    /// </summary>
    public class ScoreViewModel : ObservableObject
    {
        private readonly IExamService _examService;
        private readonly IGradingService _gradingService;
        private readonly IDialogService _dialogService;

        private ExamRecord? _examRecord;
        public ExamRecord? ExamRecord
        {
            get => _examRecord;
            set => SetProperty(ref _examRecord, value);
        }

        private ObservableCollection<AnswerRecord> _answerRecords = new();
        public ObservableCollection<AnswerRecord> AnswerRecords
        {
            get => _answerRecords;
            set => SetProperty(ref _answerRecords, value);
        }

        private double _totalScore;
        public double TotalScore
        {
            get => _totalScore;
            set => SetProperty(ref _totalScore, value);
        }

        private double _earnedScore;
        public double EarnedScore
        {
            get => _earnedScore;
            set => SetProperty(ref _earnedScore, value);
        }

        private bool _isPassed;
        public bool IsPassed
        {
            get => _isPassed;
            set => SetProperty(ref _isPassed, value);
        }

        private int _correctCount;
        public int CorrectCount
        {
            get => _correctCount;
            set => SetProperty(ref _correctCount, value);
        }

        private int _wrongCount;
        public int WrongCount
        {
            get => _wrongCount;
            set => SetProperty(ref _wrongCount, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ScoreViewModel(
            IExamService examService,
            IGradingService gradingService,
            IDialogService dialogService)
        {
            _examService = examService;
            _gradingService = gradingService;
            _dialogService = dialogService;
        }

        public void Initialize(ExamRecord examRecord)
        {
            ExamRecord = examRecord;
            _ = LoadScoreDetailsAsync();
        }

        private async Task LoadScoreDetailsAsync()
        {
            IsLoading = true;
            try
            {
                if (ExamRecord == null) return;

                // 加载答题记录
                var answers = await _examService.GetAnswerRecordsAsync(ExamRecord.Id);
                AnswerRecords = new ObservableCollection<AnswerRecord>(answers);

                // 计算统计信息
                TotalScore = ExamRecord.ExamPaper?.TotalScore ?? 0;
                EarnedScore = ExamRecord.TotalScore;
                IsPassed = EarnedScore >= (ExamRecord.ExamPaper?.PassingScore ?? 60);
                
                CorrectCount = 0;
                WrongCount = 0;
                foreach (var answer in AnswerRecords)
                {
                    if (answer.IsCorrect)
                        CorrectCount++;
                    else
                        WrongCount++;
                }
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载成绩详情失败: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
