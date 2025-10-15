using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSystem.Services.Interfaces;

namespace ExamSystem.ViewModels
{
    /// <summary>
    /// 首页视图模型
    /// </summary>
    public class HomeViewModel : ObservableObject
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IUserService _userService;

        private int _totalUsers;
        public int TotalUsers
        {
            get => _totalUsers;
            set => SetProperty(ref _totalUsers, value);
        }

        private int _totalQuestionBanks;
        public int TotalQuestionBanks
        {
            get => _totalQuestionBanks;
            set => SetProperty(ref _totalQuestionBanks, value);
        }

        private int _totalExamPapers;
        public int TotalExamPapers
        {
            get => _totalExamPapers;
            set => SetProperty(ref _totalExamPapers, value);
        }

        private int _totalExams;
        public int TotalExams
        {
            get => _totalExams;
            set => SetProperty(ref _totalExams, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public HomeViewModel(IStatisticsService statisticsService, IUserService userService)
        {
            _statisticsService = statisticsService;
            _userService = userService;
            
            _ = LoadStatisticsAsync();
        }

        private async Task LoadStatisticsAsync()
        {
            IsLoading = true;
            try
            {
                // 加载统计数据
                // TODO: 根据实际服务实现调用相应方法
                TotalUsers = 0;
                TotalQuestionBanks = 0;
                TotalExamPapers = 0;
                TotalExams = 0;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
