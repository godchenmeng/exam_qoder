using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExamSystem.UI.Controls
{
    /// <summary>
    /// 分页控件
    /// </summary>
    public class PaginationControl : Control
    {
        static PaginationControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PaginationControl), 
                new FrameworkPropertyMetadata(typeof(PaginationControl)));
        }

        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register(nameof(CurrentPage), typeof(int), typeof(PaginationControl),
                new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPageChanged));

        public int CurrentPage
        {
            get => (int)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        public static readonly DependencyProperty TotalPagesProperty =
            DependencyProperty.Register(nameof(TotalPages), typeof(int), typeof(PaginationControl),
                new PropertyMetadata(1, OnPageChanged));

        public int TotalPages
        {
            get => (int)GetValue(TotalPagesProperty);
            set => SetValue(TotalPagesProperty, value);
        }

        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register(nameof(PageSize), typeof(int), typeof(PaginationControl),
                new PropertyMetadata(10));

        public int PageSize
        {
            get => (int)GetValue(PageSizeProperty);
            set => SetValue(PageSizeProperty, value);
        }

        public static readonly DependencyProperty TotalCountProperty =
            DependencyProperty.Register(nameof(TotalCount), typeof(int), typeof(PaginationControl),
                new PropertyMetadata(0));

        public int TotalCount
        {
            get => (int)GetValue(TotalCountProperty);
            set => SetValue(TotalCountProperty, value);
        }

        public static readonly RoutedEvent PageChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(PageChanged), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(PaginationControl));

        public event RoutedEventHandler PageChanged
        {
            add => AddHandler(PageChangedEvent, value);
            remove => RemoveHandler(PageChangedEvent, value);
        }

        private static void OnPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PaginationControl)d;
            control.RaiseEvent(new RoutedEventArgs(PageChangedEvent));
        }

        public ICommand FirstPageCommand => new RelayCommand(() => CurrentPage = 1, () => CurrentPage > 1);
        public ICommand PreviousPageCommand => new RelayCommand(() => CurrentPage--, () => CurrentPage > 1);
        public ICommand NextPageCommand => new RelayCommand(() => CurrentPage++, () => CurrentPage < TotalPages);
        public ICommand LastPageCommand => new RelayCommand(() => CurrentPage = TotalPages, () => CurrentPage < TotalPages);

        private class RelayCommand : ICommand
        {
            private readonly Action _execute;
            private readonly Func<bool> _canExecute;

            public RelayCommand(Action execute, Func<bool> canExecute = null!)
            {
                _execute = execute;
                _canExecute = canExecute;
            }

            public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
            public void Execute(object? parameter) => _execute();
            public event EventHandler? CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }
        }
    }
}
