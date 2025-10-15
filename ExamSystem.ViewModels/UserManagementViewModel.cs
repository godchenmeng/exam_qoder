using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Enums;
using ExamSystem.Services.Interfaces;
using ExamSystem.UI.Services;

namespace ExamSystem.ViewModels
{
    /// <summary>
    /// 用户管理视图模型
    /// </summary>
    public class UserManagementViewModel : ObservableObject
    {
        private readonly IUserService _userService;
        private readonly IDialogService _dialogService;
        private readonly INotificationService _notificationService;

        private ObservableCollection<User> _users = new();
        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        private User? _selectedUser;
        public User? SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

        private string _searchKeyword = string.Empty;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                if (SetProperty(ref _searchKeyword, value))
                {
                    _ = SearchAsync();
                }
            }
        }

        private UserRole? _selectedRole;
        public UserRole? SelectedRole
        {
            get => _selectedRole;
            set
            {
                if (SetProperty(ref _selectedRole, value))
                {
                    _ = LoadUsersAsync();
                }
            }
        }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    _ = LoadUsersAsync();
                }
            }
        }

        private int _pageSize = 20;
        public int PageSize
        {
            get => _pageSize;
            set => SetProperty(ref _pageSize, value);
        }

        private int _totalCount;
        public int TotalCount
        {
            get => _totalCount;
            set => SetProperty(ref _totalCount, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public UserManagementViewModel(
            IUserService userService,
            IDialogService dialogService,
            INotificationService notificationService)
        {
            _userService = userService;
            _dialogService = dialogService;
            _notificationService = notificationService;

            _ = LoadUsersAsync();
        }

        private IAsyncRelayCommand? _loadUsersCommand;
        public IAsyncRelayCommand LoadUsersCommand => _loadUsersCommand ??= new AsyncRelayCommand(LoadUsersAsync);

        private async Task LoadUsersAsync()
        {
            IsLoading = true;
            try
            {
                var result = await _userService.GetUsersAsync(CurrentPage, PageSize);
                Users = new ObservableCollection<User>(result.Items);
                TotalCount = result.TotalCount;
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载用户列表失败: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private IAsyncRelayCommand? _searchCommand;
        public IAsyncRelayCommand SearchCommand => _searchCommand ??= new AsyncRelayCommand(SearchAsync);

        private async Task SearchAsync()
        {
            CurrentPage = 1;
            await LoadUsersAsync();
        }

        private IAsyncRelayCommand? _deleteUserCommand;
        public IAsyncRelayCommand DeleteUserCommand => _deleteUserCommand ??= new AsyncRelayCommand(DeleteUserAsync, () => SelectedUser != null);

        private async Task DeleteUserAsync()
        {
            if (SelectedUser == null) return;

            var confirmed = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除用户 {SelectedUser.Username} 吗？");
            if (!confirmed) return;

            try
            {
                await _userService.DeleteUserAsync(SelectedUser.Id);
                _notificationService.ShowSuccess("用户删除成功");
                await LoadUsersAsync();
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"删除用户失败: {ex.Message}");
            }
        }
    }
}
