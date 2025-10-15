using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ExamSystem.UI.Controls
{
    /// <summary>
    /// 倒计时控件
    /// </summary>
    public class CountdownTimer : Control
    {
        private DispatcherTimer? _timer;

        static CountdownTimer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CountdownTimer), 
                new FrameworkPropertyMetadata(typeof(CountdownTimer)));
        }

        public static readonly DependencyProperty RemainingSecondsProperty =
            DependencyProperty.Register(nameof(RemainingSeconds), typeof(int), typeof(CountdownTimer),
                new PropertyMetadata(0, OnRemainingSecondsChanged));

        public int RemainingSeconds
        {
            get => (int)GetValue(RemainingSecondsProperty);
            set => SetValue(RemainingSecondsProperty, value);
        }

        public static readonly DependencyProperty WarningThresholdProperty =
            DependencyProperty.Register(nameof(WarningThreshold), typeof(int), typeof(CountdownTimer),
                new PropertyMetadata(300)); // 默认5分钟警告

        public int WarningThreshold
        {
            get => (int)GetValue(WarningThresholdProperty);
            set => SetValue(WarningThresholdProperty, value);
        }

        public static readonly DependencyProperty ShowProgressRingProperty =
            DependencyProperty.Register(nameof(ShowProgressRing), typeof(bool), typeof(CountdownTimer),
                new PropertyMetadata(false));

        public bool ShowProgressRing
        {
            get => (bool)GetValue(ShowProgressRingProperty);
            set => SetValue(ShowProgressRingProperty, value);
        }

        public static readonly DependencyProperty FormattedTimeProperty =
            DependencyProperty.Register(nameof(FormattedTime), typeof(string), typeof(CountdownTimer),
                new PropertyMetadata("00:00:00"));

        public string FormattedTime
        {
            get => (string)GetValue(FormattedTimeProperty);
            private set => SetValue(FormattedTimeProperty, value);
        }

        public static readonly DependencyProperty IsWarningProperty =
            DependencyProperty.Register(nameof(IsWarning), typeof(bool), typeof(CountdownTimer),
                new PropertyMetadata(false));

        public bool IsWarning
        {
            get => (bool)GetValue(IsWarningProperty);
            private set => SetValue(IsWarningProperty, value);
        }

        public static readonly RoutedEvent TimeUpEvent =
            EventManager.RegisterRoutedEvent(nameof(TimeUp), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(CountdownTimer));

        public event RoutedEventHandler TimeUp
        {
            add => AddHandler(TimeUpEvent, value);
            remove => RemoveHandler(TimeUpEvent, value);
        }

        public static readonly RoutedEvent WarningEvent =
            EventManager.RegisterRoutedEvent(nameof(Warning), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(CountdownTimer));

        public event RoutedEventHandler Warning
        {
            add => AddHandler(WarningEvent, value);
            remove => RemoveHandler(WarningEvent, value);
        }

        private static void OnRemainingSecondsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CountdownTimer)d;
            control.UpdateFormattedTime();
            control.CheckWarning();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            StartTimer();
        }

        private void StartTimer()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (RemainingSeconds > 0)
            {
                RemainingSeconds--;
            }
            else
            {
                _timer?.Stop();
                RaiseEvent(new RoutedEventArgs(TimeUpEvent));
            }
        }

        private void UpdateFormattedTime()
        {
            var timeSpan = TimeSpan.FromSeconds(RemainingSeconds);
            FormattedTime = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }

        private void CheckWarning()
        {
            if (RemainingSeconds <= WarningThreshold && RemainingSeconds > 0 && !IsWarning)
            {
                IsWarning = true;
                RaiseEvent(new RoutedEventArgs(WarningEvent));
            }
            else if (RemainingSeconds > WarningThreshold)
            {
                IsWarning = false;
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == RemainingSecondsProperty)
            {
                UpdateFormattedTime();
            }
        }
    }
}
