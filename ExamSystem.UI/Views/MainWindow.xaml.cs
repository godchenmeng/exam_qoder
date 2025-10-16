using System.Windows;
using System.Windows.Controls;
using ExamSystem.Domain.Enums;
using ExamSystem.Abstractions.Services;
using ExamSystem.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using MaterialDesignThemes.Wpf;

namespace ExamSystem.UI.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private readonly INavigationService _navigationService;

        public MainWindow()
        {
            InitializeComponent();
            
            var app = (App)Application.Current;
            _viewModel = app.ServiceProvider.GetRequiredService<MainViewModel>();
            _navigationService = app.ServiceProvider.GetRequiredService<INavigationService>();
            DataContext = _viewModel;

            // 初始化用户信息（从登录窗口传递）
            // TODO: 从服务中获取当前用户信息

            // 订阅用户信息变化，确保登录后菜单能正确加载
            _viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.CurrentUser))
                {
                    LoadNavigationMenu();
                }
            };

            // 订阅导航服务的当前视图模型变化，使右侧内容区域随导航更新
            _navigationService.CurrentViewModelChanged += (sender, currentVm) =>
            {
                _viewModel.CurrentViewModel = currentVm!;
            };

            // 初次加载时，如果尚未有用户则菜单为空；登录后会触发加载
            LoadNavigationMenu();
        }

        private void LoadNavigationMenu()
        {
            NavigationMenu.Children.Clear();

            if (_viewModel.CurrentUser == null)
                return;

            // 根据用户角色加载不同的菜单项
            switch (_viewModel.CurrentUser.Role)
            {
                case UserRole.Admin:
                    AddMenuItem("Home", "首页", PackIconKind.Home);
                    AddMenuItem("UserManagement", "用户管理", PackIconKind.AccountMultiple);
                    AddMenuItem("QuestionBank", "题库管理", PackIconKind.BookOpenPageVariant);
                    AddMenuItem("ExamPaper", "试卷管理", PackIconKind.FileDocument);
                    AddMenuItem("ExamMonitor", "考试监控", PackIconKind.Monitor);
                    AddMenuItem("Grading", "评分管理", PackIconKind.ClipboardCheck);
                    AddMenuItem("Statistics", "统计分析", PackIconKind.ChartBar);
                    AddMenuItem("Settings", "系统设置", PackIconKind.Cog);
                    break;

                case UserRole.Teacher:
                    AddMenuItem("Home", "首页", PackIconKind.Home);
                    AddMenuItem("QuestionBank", "题库管理", PackIconKind.BookOpenPageVariant);
                    AddMenuItem("ExamPaper", "试卷管理", PackIconKind.FileDocument);
                    AddMenuItem("ExamMonitor", "考试监控", PackIconKind.Monitor);
                    AddMenuItem("Grading", "评分管理", PackIconKind.ClipboardCheck);
                    AddMenuItem("Statistics", "统计分析", PackIconKind.ChartBar);
                    break;

                case UserRole.Student:
                    AddMenuItem("Home", "首页", PackIconKind.Home);
                    AddMenuItem("ExamList", "考试列表", PackIconKind.ClipboardList);
                    AddMenuItem("Score", "我的成绩", PackIconKind.Trophy);
                    AddMenuItem("WrongQuestion", "错题本", PackIconKind.BookAlert);
                    break;
            }
        }

        private void AddMenuItem(string destination, string text, PackIconKind icon)
        {
            var button = new Button
            {
                Style = Application.Current.Resources["MaterialDesignFlatButton"] as Style,
                Height = 48,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Command = _viewModel.NavigateCommand,
                CommandParameter = destination
            };

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            var packIcon = new PackIcon
            {
                Kind = icon,
                Width = 24,
                Height = 24,
                Margin = new Thickness(16, 0, 16, 0)
            };

            var textBlock = new TextBlock
            {
                Text = text,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 14
            };

            stackPanel.Children.Add(packIcon);
            stackPanel.Children.Add(textBlock);

            button.Content = stackPanel;
            NavigationMenu.Children.Add(button);
        }
    }
}
