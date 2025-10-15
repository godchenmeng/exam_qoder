using System.Windows;
using System.Windows.Controls;
using ExamSystem.Domain.Enums;
using ExamSystem.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using MaterialDesignThemes.Wpf;

namespace ExamSystem.UI.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            
            var app = (App)Application.Current;
            _viewModel = app.ServiceProvider.GetRequiredService<MainViewModel>();
            DataContext = _viewModel;

            // 初始化用户信息（从登录窗口传递）
            // TODO: 从服务中获取当前用户信息

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
