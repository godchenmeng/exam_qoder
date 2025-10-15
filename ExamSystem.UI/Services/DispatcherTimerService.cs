using System;
using System.Windows.Threading;
using ExamSystem.Abstractions.Services;

namespace ExamSystem.UI.Services
{
    /// <summary>
    /// 基于 DispatcherTimer 的定时器服务实现
    /// </summary>
    public class DispatcherTimerService : ITimerService, IDisposable
    {
        private readonly DispatcherTimer _dispatcherTimer;
        private bool _disposed = false;

        /// <summary>
        /// 定时器触发事件
        /// </summary>
        public event EventHandler? Tick;

        /// <summary>
        /// 获取定时器是否正在运行
        /// </summary>
        public bool IsRunning => _dispatcherTimer.IsEnabled;

        /// <summary>
        /// 初始化 DispatcherTimerService 的新实例
        /// </summary>
        public DispatcherTimerService()
        {
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += OnDispatcherTimerTick;
        }

        /// <summary>
        /// 启动定时器
        /// </summary>
        /// <param name="interval">定时器间隔</param>
        public void Start(TimeSpan interval)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DispatcherTimerService));

            _dispatcherTimer.Interval = interval;
            _dispatcherTimer.Start();
        }

        /// <summary>
        /// 停止定时器
        /// </summary>
        public void Stop()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DispatcherTimerService));

            _dispatcherTimer.Stop();
        }

        /// <summary>
        /// 处理 DispatcherTimer 的 Tick 事件
        /// </summary>
        private void OnDispatcherTimerTick(object? sender, EventArgs e)
        {
            Tick?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否正在 disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dispatcherTimer.Stop();
                    _dispatcherTimer.Tick -= OnDispatcherTimerTick;
                }

                _disposed = true;
            }
        }
    }
}