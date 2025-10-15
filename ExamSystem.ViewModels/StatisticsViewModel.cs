using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using ExamSystem.Services.Interfaces;

namespace ExamSystem.ViewModels
{
    /// <summary>
    /// 统计分析视图模型
    /// </summary>
    public class StatisticsViewModel : ObservableObject
    {
        private readonly IStatisticsService _statisticsService;

        private int _totalExams;
        public int TotalExams
        {
            get => _totalExams;
            set => SetProperty(ref _totalExams, value);
        }

        private int _totalStudents;
        public int TotalStudents
        {
            get => _totalStudents;
            set => SetProperty(ref _totalStudents, value);
        }

        private double _averageScore;
        public double AverageScore
        {
            get => _averageScore;
            set => SetProperty(ref _averageScore, value);
        }

        private double _passingRate;
        public double PassingRate
        {
            get => _passingRate;
            set => SetProperty(ref _passingRate, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public StatisticsViewModel(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
            _ = LoadStatisticsAsync();
        }

        private async Task LoadStatisticsAsync()
        {
            IsLoading = true;
            try
            {
                // TODO: 实现统计数据加载
                TotalExams = 0;
                TotalStudents = 0;
                AverageScore = 0;
                PassingRate = 0;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
