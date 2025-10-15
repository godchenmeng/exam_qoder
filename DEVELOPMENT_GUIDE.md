# 在线考试系统 - 开发指南

## 快速开始

### 1. 克隆项目

```bash
cd /Users/chenmeng/c#/exam_qoder
```

### 2. 还原依赖

```bash
dotnet restore
```

### 3. 编译项目

```bash
dotnet build
```

所有项目编译成功,无警告无错误! ✅

## 项目架构

### 分层结构

```
┌─────────────────────────────────────────┐
│          ExamSystem.UI (WPF)            │  表现层
├─────────────────────────────────────────┤
│       ExamSystem.ViewModels             │  视图模型层
├─────────────────────────────────────────┤
│        ExamSystem.Services              │  业务服务层
├─────────────────────────────────────────┤
│       ExamSystem.Repository             │  数据访问层
├─────────────────────────────────────────┤
│   ExamSystem.Infrastructure (工具)      │  基础设施层
├─────────────────────────────────────────┤
│     ExamSystem.Domain (实体/DTO)        │  领域模型层
└─────────────────────────────────────────┘
```

### 项目依赖关系

```
ExamSystem.UI
  ↓ 依赖
ExamSystem.ViewModels
  ↓ 依赖
ExamSystem.Services
  ↓ 依赖
ExamSystem.Repository
  ↓ 依赖
ExamSystem.Domain

ExamSystem.Infrastructure
  ↓ 依赖
ExamSystem.Domain
```

## 已实现功能

### ✅ 领域模型层

**枚举 (Enums)**
- QuestionType: 题型(单选/多选/判断/填空/主观)
- Difficulty: 难度(简单/中等/困难)
- UserRole: 角色(管理员/教师/学生)
- PaperType: 试卷类型(固定/随机/混合)
- ExamStatus: 考试状态(进行中/已提交/已评分/超时)
- PaperStatus: 试卷状态(草稿/已激活/已归档)

**实体 (Entities)**
- User: 用户
- QuestionBank: 题库
- Question: 题目
- Option: 选项
- ExamPaper: 试卷
- PaperQuestion: 试卷题目关联
- ExamRecord: 考试记录
- AnswerRecord: 答题记录

**DTO (Data Transfer Objects)**
- UserLoginResult: 登录结果
- ValidationResult: 验证结果
- PagedResult<T>: 分页结果

### ✅ 基础设施层

**工具类 (Utils)**
- PasswordHelper: 密码加密工具
  - HashPassword: SHA256加密
  - VerifyPassword: 密码验证
  - GenerateRandomPassword: 随机密码生成
  - ValidatePasswordStrength: 密码强度验证

- JsonHelper: JSON序列化工具
  - Serialize: 对象序列化
  - Deserialize: 对象反序列化
  - TryDeserialize: 安全反序列化

- AnswerComparer: 答案比较工具
  - ExactMatch: 精确匹配
  - FuzzyMatch: 模糊匹配(Levenshtein算法)
  - CompareMultipleChoice: 多选题比较

**公共类 (Common)**
- SystemConfig: 系统配置
- Constants: 常量定义

### ✅ 数据访问层

**DbContext**
- ExamSystemDbContext: EF Core数据库上下文
  - 配置了所有实体的映射关系
  - 配置了外键和级联删除规则
  - 包含默认管理员账户种子数据

**默认账户**
- 用户名: admin
- 密码: admin123
- 角色: 管理员

## 数据库设计

### 核心表

| 表名 | 说明 | 主要字段 |
|------|------|----------|
| Users | 用户表 | UserId, Username, PasswordHash, Role |
| QuestionBanks | 题库表 | BankId, Name, CreatorId |
| Questions | 题目表 | QuestionId, BankId, QuestionType, Content, Answer |
| Options | 选项表 | OptionId, QuestionId, Content, IsCorrect |
| ExamPapers | 试卷表 | PaperId, Name, TotalScore, Duration, PaperType |
| PaperQuestions | 试卷题目关联 | PaperId, QuestionId, OrderIndex, Score |
| ExamRecords | 考试记录表 | RecordId, UserId, PaperId, Status, TotalScore |
| AnswerRecords | 答题记录表 | AnswerId, RecordId, QuestionId, UserAnswer, Score |

### 关系图 ──1:N──> QuestionBanks
Users ──1:N──> ExamPapers
Users ──1:N──> ExamRecords

QuestionBanks ──1:N──> Questions
Questions ──1:N──> Options

ExamPapers ──1:N──> PaperQuestions
Questions ──1:N──> PaperQuestions

ExamPapers ──1:N──> ExamRecords
ExamRecords ──1:N──> AnswerRecords
Questions ──1:N──> AnswerRecords
```

## 下一步开发任务

### 1. 完善数据访问层

创建Repository接口和实现:

```csharp
// 通用仓储接口
public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<int> SaveChangesAsync();
}

// 专用仓储示例
public interface IUserRepository : IRepository<User>
{
    Task<User> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role);
}
```

### 2. 实现服务层

创建核心业务服务:

```csharp
// 用户服务
public interface IUserService
{
    Task<UserLoginResult> LoginAsync(string username, string password);
    Task<User> CreateUserAsync(User user);
    Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
}

// 题目服务
public interface IQuestionService
{
    Task<Question> CreateQuestionAsync(Question question);
    Task<PagedResult<Question>> GetQuestionsByBankAsync(int bankId, int page, int pageSize);
    Task<ValidationResult> ValidateQuestionAsync(Question question);
}

// 试卷服务
public interface IExamPaperService
{
    Task<ExamPaper> CreateFixedPaperAsync(ExamPaper paper, List<int> questionIds);
    Task<ExamPaper> CreateRandomPaperAsync(ExamPaper paper, RandomConfig config);
}

// 考试服务
public interface IExamService
{
    Task<ExamRecord> StartExamAsync(int userId, int paperId);
    Task<bool> SaveAnswerAsync(int recordId, int questionId, string answer);
    Task<bool> SubmitExamAsync(int recordId);
}

// 评分服务
public interface IGradingService
{
    Task<bool> AutoGradeObjectiveAsync(int recordId);
    Task<bool> GradeSubjectiveAsync(int answerId, decimal score, string comment);
}
```

### 3. 实现ViewModel层

使用CommunityToolkit.Mvvm创建ViewModels:

```bash
dotnet add ExamSystem.ViewModels package CommunityToolkit.Mvvm
```

```csharp
public class LoginViewModel : ObservableObject
{
    private string _username;
    private string _password;
    
    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }
    
    [RelayCommand]
    private async Task LoginAsync()
    {
        // 登录逻辑
    }
}
```

### 4. 实现UI层

将ExamSystem.UI转换为WPF应用程序:

修改 `ExamSystem.UI.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net5.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
  </PropertyGroup>
</Project>
```

创建App.xaml和MainWindow.xaml。

### 5. 配置依赖注入

在App.xaml.cs中配置DI容器:

```csharp
public partial class App : Application
{
    private IServiceProvider _serviceProvider;
    
    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();
        
        // 注册DbContext
        services.AddDbContext<ExamSystemDbContext>(options =>
            options.UseSqlite("Data Source=ExamSystem.db"));
        
        // 注册Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        
        // 注册Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IQuestionService, QuestionService>();
        
        // 注册ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<MainViewModel>();
        
        _serviceProvider = services.BuildServiceProvider();
        
        var mainWindow = new MainWindow
        {
            DataContext = _serviceProvider.GetRequiredService<MainViewModel>()
        };
        mainWindow.Show();
    }
}
```

### 6. 数据库迁移

```bash
# 安装EF Core工具
dotnet tool install --global dotnet-ef

# 添加迁移
dotnet ef migrations add InitialCreate --project ExamSystem.Repository

# 更新数据库
dotnet ef database update --project ExamSystem.Repository
```

## 开发规范

### 命名规范

- 类名: PascalCase (如: `UserService`)
- 方法名: PascalCase (如: `GetUserById`)
- 参数名: camelCase (如: `userId`)
- 私有字段: _camelCase (如: `_userName`)
- 常量: PascalCase (如: `DefaultPageSize`)

### 异步编程

所有I/O操作使用异步方法:

```csharp
// ✅ 正确
public async Task<User> GetUserAsync(int id)
{
    return await _repository.GetByIdAsync(id);
}

// ❌ 错误
public User GetUser(int id)
{
    return _repository.GetById(id);
}
```

### 异常处理

```csharp
public async Task<UserLoginResult> LoginAsync(string username, string password)
{
    try
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        
        if (user == null || !PasswordHelper.VerifyPassword(password, user.PasswordHash))
        {
            return new UserLoginResult
            {
                Success = false,
                ErrorMessage = "用户名或密码错误"
            };
        }
        
        return new UserLoginResult
        {
            Success = true,
            UserId = user.UserId,
            Username = user.Username,
            Role = user.Role
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Login failed for user: {Username}", username);
        throw;
    }
}
```

### 事务处理

关键业务操作使用事务:

```csharp
public async Task<ExamPaper> CreateFixedPaperAsync(ExamPaper paper, List<int> questionIds)
{
    using var transaction = await _context.Database.BeginTransactionAsync();
    
    try
    {
        // 保存试卷
        await _context.ExamPapers.AddAsync(paper);
        await _context.SaveChangesAsync();
        
        // 保存试卷题目关联
        int order = 1;
        foreach (var questionId in questionIds)
        {
            var paperQuestion = new PaperQuestion
            {
                PaperId = paper.PaperId,
                QuestionId = questionId,
                OrderIndex = order++
            };
            await _context.PaperQuestions.AddAsync(paperQuestion);
        }
        
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        
        return paper;
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

## 测试指南

### 单元测试示例

```csharp
public class PasswordHelperTests
{
    [Fact]
    public void HashPassword_ShouldReturnNonEmptyString()
    {
        // Arrange
        var password = "test123";
        
        // Act
        var hash = PasswordHelper.HashPassword(password);
        
        // Assert
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
    }
    
    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "test123";
        var hash = PasswordHelper.HashPassword(password);
        
        // Act
        var result = PasswordHelper.VerifyPassword(password, hash);
        
        // Assert
        Assert.True(result);
    }
}
```

## 常见问题

### 1. 如何添加新的实体?

1. 在`ExamSystem.Domain/Entities`中创建实体类
2. 在`ExamSystemDbContext`中添加`DbSet<T>`
3. 在`OnModelCreating`中配置实体
4. 添加迁移并更新数据库

### 2. 如何实现新的业务功能?

1. 在Service层定义接口和实现
2. 在ViewModel层调用Service
3. 在View层绑定ViewModel

### 3. 如何调试数据库查询?

在`appsettings.json`中启用日志:

```json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

## 参考资料

- [Entity Framework Core文档](https://docs.microsoft.com/ef/core/)
- [WPF文档](https://docs.microsoft.com/dotnet/desktop/wpf/)
- [MVVM Toolkit](https://docs.microsoft.com/windows/communitytoolkit/mvvm/introduction)

## 联系方式

如有问题,请参考项目文档或查看代码注释。
