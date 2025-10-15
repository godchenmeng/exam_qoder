using System;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ExamSystem.UI.Services;
using ExamSystem.Abstractions.Services;
using ExamSystem.ViewModels;
using ExamSystem.Services.Interfaces;
using ExamSystem.Services.Implementations;
using ExamSystem.Repository.Interfaces;
using ExamSystem.Repository.Repositories;
using ExamSystem.Repository.Context;

namespace ExamSystem.UI
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; } = null!;
        public IConfiguration Configuration { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 配置Serilog
            ConfigureLogging();

            try
            {
                // 加载配置
                Configuration = BuildConfiguration();

                // 配置依赖注入
                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                ServiceProvider = serviceCollection.BuildServiceProvider();

                // 初始化数据库
                InitializeDatabase();

                Log.Information("应用程序启动成功");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "应用程序启动失败");
                MessageBox.Show($"应用程序启动失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("Logs/exam-system-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        private IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // 配置DbContext
            services.AddDbContext<ExamSystemDbContext>(options =>
            {
                var connectionString = Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlite(connectionString);
            });

            // 注册Repository
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IQuestionBankRepository, QuestionBankRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IExamPaperRepository, ExamPaperRepository>();
            services.AddScoped<IExamRecordRepository, ExamRecordRepository>();
            services.AddScoped<IAnswerRecordRepository, AnswerRecordRepository>();

            // 注册Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IExamPaperService, ExamPaperService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IGradingService, GradingService>();
            services.AddScoped<IStatisticsService, StatisticsService>();

            // 注册UI服务
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IFileDialogService, FileDialogService>();
            
            // 注册定时器服务
            services.AddTransient<ITimerService, DispatcherTimerService>();

            // 注册ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<UserManagementViewModel>();
            services.AddTransient<SystemSettingsViewModel>();
            services.AddTransient<QuestionBankViewModel>();
            services.AddTransient<ExamPaperViewModel>();
            services.AddTransient<StatisticsViewModel>();
            services.AddTransient<ExamListViewModel>();
            services.AddTransient<ExamTakingViewModel>();
            services.AddTransient<ScoreViewModel>();

            Log.Information("依赖注入配置完成");
        }

        private void InitializeDatabase()
        {
            using var scope = ServiceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ExamSystemDbContext>();
            
            // 确保数据库创建
            dbContext.Database.EnsureCreated();
            
            // 初始化种子数据
            DbInitializer.Initialize(dbContext);
            
            Log.Information("数据库初始化完成");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("应用程序退出");
            Log.CloseAndFlush();
            base.OnExit(e);
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error(e.Exception, "未处理的异常");
            MessageBox.Show($"发生错误: {e.Exception.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
