using System.Windows;
using System.Windows.Controls;
using ExamSystem.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ExamSystem.UI.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginWindow()
        {
            InitializeComponent();
            
            // 从DI容器获取ViewModel
            var app = (App)Application.Current;
            _viewModel = app.ServiceProvider.GetRequiredService<LoginViewModel>();
            DataContext = _viewModel;

            // 订阅登录成功事件
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LoginViewModel.LoginResult))
            {
                if (_viewModel.LoginResult != null && _viewModel.LoginResult.Success)
                {
                    // 登录成功，打开主窗口
                    var app = (App)Application.Current;
                    var mainWindow = new MainWindow();
                    // 将登录结果传递给主窗口的 ViewModel
                    if (mainWindow.DataContext is MainViewModel mainVm)
                    {
                        mainVm.Initialize(_viewModel.LoginResult);
                    }
                    mainWindow.Show();
                    this.Close();
                }
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                _viewModel.Password = passwordBox.Password;
            }
        }
    }
}
