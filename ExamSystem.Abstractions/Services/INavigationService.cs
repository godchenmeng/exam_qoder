using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ExamSystem.Abstractions.Services
{
    /// <summary>
    /// 导航服务接口
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// 导航到指定的ViewModel
        /// </summary>
        void NavigateTo<TViewModel>() where TViewModel : ObservableObject;

        /// <summary>
        /// 带参数导航到指定的ViewModel
        /// </summary>
        void NavigateTo<TViewModel>(object parameter) where TViewModel : ObservableObject;

        /// <summary>
        /// 导航到指定类型的ViewModel
        /// </summary>
        void NavigateTo(Type viewModelType);

        /// <summary>
        /// 返回上一页
        /// </summary>
        void GoBack();

        /// <summary>
        /// 是否可以返回
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        /// 当前ViewModel
        /// </summary>
        ObservableObject? CurrentViewModel { get; }

        /// <summary>
        /// ViewModel改变事件
        /// </summary>
        event EventHandler<ObservableObject?>? CurrentViewModelChanged;
    }
}
