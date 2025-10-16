using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSystem.Abstractions.Services;
using ExamSystem.Domain.DTOs;
using ExamSystem.Domain.Enums;

namespace ExamSystem.ViewModels
{
    /// <summary>
    /// 主窗口视图模型
    /// </summary>
    public class MainViewModel : ObservableObject
    {
        private UserLoginResult _currentUser;
        public UserLoginResult CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        private string _title = "在线考试系统";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private ObservableObject _currentViewModel;
        public ObservableObject CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        private bool _isMenuExpanded = true;
        public bool IsMenuExpanded
        {
            get => _isMenuExpanded;
            set => SetProperty(ref _isMenuExpanded, value);
        }

        private readonly INavigationService _navigationService;

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        /// <summary>
        /// 初始化用户信息
        /// </summary>
        public void Initialize(UserLoginResult userLoginResult)
        {
            CurrentUser = userLoginResult;
            Title = $"在线考试系统 - {CurrentUser.RealName} ({GetRoleName(CurrentUser.Role)})";

            // 登录成功后默认导航到首页
            _navigationService.NavigateTo<HomeViewModel>();
        }

        private IRelayCommand<string> _navigateCommand;
        public IRelayCommand<string> NavigateCommand => _navigateCommand ??= new RelayCommand<string>(Navigate);

        /// <summary>
        /// 导航命令
        /// </summary>
        private void Navigate(string destination)
        {
            // 根据目标页面创建对应的ViewModel
            // 实际应用中应该通过DI容器来创建
            switch (destination)
            {
                case "Home":
                    _navigationService.NavigateTo<HomeViewModel>();
                    break;
                case "UserManagement":
                    _navigationService.NavigateTo<UserManagementViewModel>();
                    break;
                case "QuestionBank":
                    _navigationService.NavigateTo<QuestionBankViewModel>();
                    break;
                case "Settings":
                    _navigationService.NavigateTo<SystemSettingsViewModel>();
                    break;
                case "ExamPaper":
                    _navigationService.NavigateTo<ExamPaperViewModel>();
                    break;
                case "Statistics":
                    _navigationService.NavigateTo<StatisticsViewModel>();
                    break;
                case "ExamList":
                    _navigationService.NavigateTo<ExamListViewModel>();
                    break;
                case "Score":
                    _navigationService.NavigateTo<ScoreViewModel>();
                    break;
                default:
                    break;
            }
        }

        private IRelayCommand _toggleMenuCommand;
        public IRelayCommand ToggleMenuCommand => _toggleMenuCommand ??= new RelayCommand(ToggleMenu);

        /// <summary>
        /// 切换菜单展开状态
        /// </summary>
        private void ToggleMenu()
        {
            IsMenuExpanded = !IsMenuExpanded;
        }

        private IRelayCommand _logoutCommand;
        public IRelayCommand LogoutCommand => _logoutCommand ??= new RelayCommand(Logout);

        /// <summary>
        /// 退出登录
        /// </summary>
        private void Logout()
        {
            // 清除当前用户信息
            CurrentUser = null;
            // 导航回登录页面
        }

        private string GetRoleName(UserRole role)
        {
            return role switch
            {
                UserRole.Admin => "管理员",
                UserRole.Teacher => "教师",
                UserRole.Student => "学生",
                _ => "未知"
            };
        }
    }
}
