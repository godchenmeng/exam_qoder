using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Enums;
using ExamSystem.Services.Interfaces;
using ExamSystem.Abstractions.Services;
using System.Collections.Generic;
using System;

namespace ExamSystem.ViewModels
{
    /// <summary>
    /// 题库管理视图模型
    /// </summary>
    public class QuestionBankViewModel : ObservableObject
    {
        private readonly IQuestionService _questionService;
        private readonly IDialogService _dialogService;
        private readonly INotificationService _notificationService;

        private ObservableCollection<QuestionBank> _questionBanks = new();
        public ObservableCollection<QuestionBank> QuestionBanks
        {
            get => _questionBanks;
            set => SetProperty(ref _questionBanks, value);
        }

        private QuestionBank? _selectedBank;
        public QuestionBank? SelectedBank
        {
            get => _selectedBank;
            set
            {
                if (SetProperty(ref _selectedBank, value))
                {
                    _ = LoadQuestionsAsync();
                }
            }
        }

        private ObservableCollection<Question> _questions = new();
        public ObservableCollection<Question> Questions
        {
            get => _questions;
            set => SetProperty(ref _questions, value);
        }

        private Question? _selectedQuestion;
        public Question? SelectedQuestion
        {
            get => _selectedQuestion;
            set => SetProperty(ref _selectedQuestion, value);
        }

        private QuestionType? _questionTypeFilter;
        public QuestionType? QuestionTypeFilter
        {
            get => _questionTypeFilter;
            set
            {
                if (SetProperty(ref _questionTypeFilter, value))
                {
                    _ = LoadQuestionsAsync();
                }
            }
        }

        private Difficulty? _difficultyFilter;
        public Difficulty? DifficultyFilter
        {
            get => _difficultyFilter;
            set
            {
                if (SetProperty(ref _difficultyFilter, value))
                {
                    _ = LoadQuestionsAsync();
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // 添加枚举数据源属性
        public IEnumerable<QuestionType> QuestionTypes => Enum.GetValues<QuestionType>();
        public IEnumerable<Difficulty> Difficulties => Enum.GetValues<Difficulty>();

        public QuestionBankViewModel(
            IQuestionService questionService,
            IDialogService dialogService,
            INotificationService notificationService)
        {
            _questionService = questionService;
            _dialogService = dialogService;
            _notificationService = notificationService;

            _ = LoadQuestionBanksAsync();
        }

        #region 题库操作命令

        private IAsyncRelayCommand? _loadQuestionBanksCommand;
        public IAsyncRelayCommand LoadQuestionBanksCommand => 
            _loadQuestionBanksCommand ??= new AsyncRelayCommand(LoadQuestionBanksAsync);

        private async Task LoadQuestionBanksAsync()
        {
            IsLoading = true;
            try
            {
                var banks = await _questionService.GetAllQuestionBanksAsync();
                QuestionBanks = new ObservableCollection<QuestionBank>(banks);
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载题库列表失败: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private IAsyncRelayCommand? _createBankCommand;
        public IAsyncRelayCommand CreateBankCommand => 
            _createBankCommand ??= new AsyncRelayCommand(CreateBankAsync);

        private async Task CreateBankAsync()
        {
            try
            {
                // 这里应该打开一个创建题库的对话框
                // 暂时使用简单的输入对话框
                var bankName = await _dialogService.ShowInputDialogAsync("新建题库", "请输入题库名称:");
                if (string.IsNullOrWhiteSpace(bankName)) return;

                var description = await _dialogService.ShowInputDialogAsync("新建题库", "请输入题库描述:");

                var newBank = new QuestionBank
                {
                    Name = bankName,
                    Description = description ?? "",
                    CreatedAt = DateTime.Now,
                    CreatorId = 1 // 暂时硬编码，实际应该从当前用户获取
                };

                // 注意：这里需要题库服务支持创建操作
                // 由于当前 IQuestionService 没有创建题库的方法，我们先显示提示
                _notificationService.ShowInfo("创建题库功能需要完善题库服务接口");
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"创建题库失败: {ex.Message}");
            }
        }

        #endregion

        #region 题目操作命令

        private IAsyncRelayCommand? _loadQuestionsCommand;
        public IAsyncRelayCommand LoadQuestionsCommand => 
            _loadQuestionsCommand ??= new AsyncRelayCommand(LoadQuestionsAsync);

        private async Task LoadQuestionsAsync()
        {
            if (SelectedBank == null)
            {
                Questions.Clear();
                return;
            }

            IsLoading = true;
            try
            {
                // 修复页码：仓储分页为 1 起始，这里传 1，避免 Skip 负数导致异常
                var questions = await _questionService.GetQuestionsByBankIdAsync(SelectedBank.BankId, 1, 10);
                
                // 应用筛选
                var filtered = questions.Items.AsEnumerable();
                if (QuestionTypeFilter.HasValue)
                {
                    filtered = filtered.Where(q => q.QuestionType == QuestionTypeFilter.Value);
                }
                if (DifficultyFilter.HasValue)
                {
                    filtered = filtered.Where(q => q.Difficulty == DifficultyFilter.Value);
                }

                Questions = new ObservableCollection<Question>(filtered);
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载题目列表失败: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private IAsyncRelayCommand? _createQuestionCommand;
        public IAsyncRelayCommand CreateQuestionCommand => 
            _createQuestionCommand ??= new AsyncRelayCommand(CreateQuestionAsync, () => SelectedBank != null);

        private async Task CreateQuestionAsync()
        {
            if (SelectedBank == null) return;

            try
            {
                // 这里应该打开一个创建题目的对话框
                // 暂时使用简单的输入对话框演示
                var content = await _dialogService.ShowInputDialogAsync("新增题目", "请输入题目内容:");
                if (string.IsNullOrWhiteSpace(content)) return;

                var answer = await _dialogService.ShowInputDialogAsync("新增题目", "请输入答案:");
                if (string.IsNullOrWhiteSpace(answer)) return;

                var newQuestion = new Question
                {
                    BankId = SelectedBank.BankId,
                    Content = content,
                    Answer = answer,
                    QuestionType = QuestionType.SingleChoice, // 默认单选
                    Difficulty = Difficulty.Medium, // 默认中等难度
                    DefaultScore = 5, // 默认5分
                    CreatedAt = DateTime.Now
                };

                var createdQuestion = await _questionService.CreateQuestionAsync(newQuestion);
                _notificationService.ShowSuccess("题目创建成功");
                await LoadQuestionsAsync();
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"创建题目失败: {ex.Message}");
            }
        }

        private IAsyncRelayCommand? _editQuestionCommand;
        public IAsyncRelayCommand EditQuestionCommand => 
            _editQuestionCommand ??= new AsyncRelayCommand(EditQuestionAsync, () => SelectedQuestion != null);

        private async Task EditQuestionAsync()
        {
            if (SelectedQuestion == null) return;

            try
            {
                // 这里应该打开一个编辑题目的对话框
                // 暂时使用简单的输入对话框演示
                var content = await _dialogService.ShowInputDialogAsync("编辑题目", "请修改题目内容:", SelectedQuestion.Content);
                if (string.IsNullOrWhiteSpace(content)) return;

                var answer = await _dialogService.ShowInputDialogAsync("编辑题目", "请修改答案:", SelectedQuestion.Answer);
                if (string.IsNullOrWhiteSpace(answer)) return;

                SelectedQuestion.Content = content;
                SelectedQuestion.Answer = answer;
                SelectedQuestion.UpdatedAt = DateTime.Now;

                var success = await _questionService.UpdateQuestionAsync(SelectedQuestion);
                if (success)
                {
                    _notificationService.ShowSuccess("题目更新成功");
                    await LoadQuestionsAsync();
                }
                else
                {
                    await _dialogService.ShowErrorAsync("错误", "题目更新失败");
                }
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"编辑题目失败: {ex.Message}");
            }
        }

        private IAsyncRelayCommand? _deleteQuestionCommand;
        public IAsyncRelayCommand DeleteQuestionCommand => 
            _deleteQuestionCommand ??= new AsyncRelayCommand(DeleteQuestionAsync, () => SelectedQuestion != null);

        private async Task DeleteQuestionAsync()
        {
            if (SelectedQuestion == null) return;

            var confirmed = await _dialogService.ShowConfirmAsync("确认删除", "确定要删除该题目吗？");
            if (!confirmed) return;

            try
            {
                // 修复：使用正确的 QuestionId 而不是 BankId
                await _questionService.DeleteQuestionAsync(SelectedQuestion.QuestionId);
                _notificationService.ShowSuccess("题目删除成功");
                await LoadQuestionsAsync();
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"删除题目失败: {ex.Message}");
            }
        }

        private IAsyncRelayCommand? _duplicateQuestionCommand;
        public IAsyncRelayCommand DuplicateQuestionCommand => 
            _duplicateQuestionCommand ??= new AsyncRelayCommand(DuplicateQuestionAsync, () => SelectedQuestion != null);

        private async Task DuplicateQuestionAsync()
        {
            if (SelectedQuestion == null) return;

            try
            {
                var duplicated = await _questionService.DuplicateQuestionAsync(SelectedQuestion.QuestionId);
                _notificationService.ShowSuccess("题目复制成功");
                await LoadQuestionsAsync();
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"复制题目失败: {ex.Message}");
            }
        }

        #endregion

        #region 导入导出命令

        private IAsyncRelayCommand? _importQuestionsCommand;
        public IAsyncRelayCommand ImportQuestionsCommand => 
            _importQuestionsCommand ??= new AsyncRelayCommand(ImportQuestionsAsync, () => SelectedBank != null);

        private async Task ImportQuestionsAsync()
        {
            if (SelectedBank == null) return;

            try
            {
                _notificationService.ShowInfo("导入功能需要完善文件对话框服务");
                // 这里需要实现文件选择和导入逻辑
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"导入题目失败: {ex.Message}");
            }
        }

        private IAsyncRelayCommand? _exportQuestionsCommand;
        public IAsyncRelayCommand ExportQuestionsCommand => 
            _exportQuestionsCommand ??= new AsyncRelayCommand(ExportQuestionsAsync, () => Questions.Any());

        private async Task ExportQuestionsAsync()
        {
            try
            {
                _notificationService.ShowInfo("导出功能需要完善文件对话框服务");
                // 这里需要实现导出逻辑
            }
            catch (System.Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"导出题目失败: {ex.Message}");
            }
        }

        #endregion
    }
}
