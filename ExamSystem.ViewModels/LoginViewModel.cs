using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSystem.Domain.DTOs;
using ExamSystem.Services.Interfaces;

namespace ExamSystem.ViewModels
{
    /// <summary>
    /// 登录视图模型
    /// </summary>
    public class LoginViewModel : ObservableObject
    {
        private readonly IUserService _userService;

        private string _username;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private bool _rememberPassword;
        public bool RememberPassword
        {
            get => _rememberPassword;
            set => SetProperty(ref _rememberPassword, value);
        }

        /// <summary>
        /// 登录结果
        /// </summary>
        public UserLoginResult LoginResult { get; private set; }

        public LoginViewModel(IUserService userService)
        {
            _userService = userService;
        }

        private IAsyncRelayCommand _loginCommand;
        public IAsyncRelayCommand LoginCommand => _loginCommand ??= new AsyncRelayCommand(LoginAsync);

        /// <summary>
        /// 登录命令
        /// </summary>
        private async Task LoginAsync()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "请输入用户名";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "请输入密码";
                return;
            }

            IsLoading = true;

            try
            {
                var result = await _userService.LoginAsync(Username, Password);

                if (result.Success)
                {
                    LoginResult = result;
                    // 登录成功,触发导航到主窗口
                    // 实际应用中应该通过事件或导航服务来处理
                }
                else
                {
                    ErrorMessage = result.ErrorMessage;
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private IRelayCommand _clearErrorCommand;
        public IRelayCommand ClearErrorCommand => _clearErrorCommand ??= new RelayCommand(ClearError);

        /// <summary>
        /// 清除错误消息
        /// </summary>
        private void ClearError()
        {
            ErrorMessage = string.Empty;
        }
    }
}
