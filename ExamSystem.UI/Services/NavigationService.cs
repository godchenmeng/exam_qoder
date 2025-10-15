using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace ExamSystem.UI.Services
{
    /// <summary>
    /// 导航服务实现
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Stack<ObservableObject> _navigationStack = new();
        private ObservableObject? _currentViewModel;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ObservableObject? CurrentViewModel
        {
            get => _currentViewModel;
            private set
            {
                if (_currentViewModel != value)
                {
                    _currentViewModel = value;
                    CurrentViewModelChanged?.Invoke(this, _currentViewModel);
                }
            }
        }

        public bool CanGoBack => _navigationStack.Count > 0;

        public event EventHandler<ObservableObject?>? CurrentViewModelChanged;

        public void NavigateTo<TViewModel>() where TViewModel : ObservableObject
        {
            NavigateTo(typeof(TViewModel));
        }

        public void NavigateTo<TViewModel>(object parameter) where TViewModel : ObservableObject
        {
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            
            // 如果ViewModel有Initialize方法，调用它
            var initMethod = viewModel.GetType().GetMethod("Initialize");
            initMethod?.Invoke(viewModel, new[] { parameter });

            Navigate(viewModel);
        }

        public void NavigateTo(Type viewModelType)
        {
            if (!typeof(ObservableObject).IsAssignableFrom(viewModelType))
            {
                throw new ArgumentException($"Type {viewModelType.Name} must inherit from ObservableObject");
            }

            var viewModel = _serviceProvider.GetRequiredService(viewModelType) as ObservableObject;
            Navigate(viewModel!);
        }

        public void GoBack()
        {
            if (CanGoBack)
            {
                CurrentViewModel = _navigationStack.Pop();
            }
        }

        private void Navigate(ObservableObject viewModel)
        {
            if (CurrentViewModel != null)
            {
                _navigationStack.Push(CurrentViewModel);
            }

            CurrentViewModel = viewModel;
        }
    }
}
