using System;

namespace ExamSystem.Abstractions.Services
{
    /// <summary>
    /// 定时器服务接口
    /// </summary>
    public interface ITimerService
    {
        /// <summary>
        /// 启动定时器
        /// </summary>
        /// <param name="interval">定时器间隔</param>
        void Start(TimeSpan interval);

        /// <summary>
        /// 停止定时器
        /// </summary>
        void Stop();

        /// <summary>
        /// 获取定时器是否正在运行
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// 定时器触发事件
        /// </summary>
        event EventHandler Tick;
    }
}