# 在线考试系统 - 快速入门指南

## 项目当前状态

✅ **已完成**：
- 数据访问层（仓储接口与实现）
- 业务服务层（试卷、考试、评分、统计服务）
- 数据库配置与种子数据
- 核心DTO模型

⚠️ **待完成**：
- UI层（WPF页面和控件）
- 视图模型层（ViewModels）
- 依赖注入配置

## 项目结构

```
ExamSystem/
├── ExamSystem.Domain/              # 领域模型层
│   ├── Entities/                   # 实体类（8个）
│   ├── Enums/                      # 枚举类型（6个）
│   └── DTOs/                       # 数据传输对象（10个）
│
├── ExamSystem.Infrastructure/      # 基础设施层
│   ├── Common/                     # 常量和配置
│   └── Utils/                      # 工具类
│       ├── PasswordHelper.cs       # 密码加密
│       ├── JsonHelper.cs           # JSON序列化
│       └── AnswerComparer.cs       # 答案比较
│
├── ExamSystem.Repository/          # 数据访问层 ✅
│   ├── Context/
│   │   ├── ExamSystemDbContext.cs  # 数据库上下文
│   │   └── DbInitializer.cs        # 数据库初始化 ✅
│   ├── Interfaces/                 # 仓储接口（8个）✅
│   └── Repositories/               # 仓储实现（7个）✅
│
├── ExamSystem.Services/            # 业务服务层 ✅
│   ├── Interfaces/                 # 服务接口（6个）✅
│   └── Implementations/            # 服务实现（4个）✅
│       ├── ExamPaperService.cs     # 试卷服务 ✅
│       ├── ExamService.cs          # 考试服务 ✅
│       ├── GradingService.cs       # 评分服务 ✅
│       └── StatisticsService.cs    # 统计服务 ✅
│
├── ExamSystem.ViewModels/          # 视图模型层 ⚠️
│   ├── LoginViewModel.cs           # 登录视图模型
│   └── MainViewModel.cs            # 主窗口视图模型
│
├── ExamSystem.UI/                  # WPF UI层 ⚠️
│   └── appsettings.json            # 配置文件 ✅
│
└── ExamSystem.Tests/               # 测试项目
    └── Infrastructure/
        └── PasswordHelperTests.cs
```

## 核心功能说明

### 1. 试卷服务 (ExamPaperService)

#### 三种组卷方式

**1.1 固定试卷（手动选题）**
```csharp
var paper = new ExamPaper 
{ 
    Name = "期末考试",
    Duration = 120,
    PassScore = 60,
    CreatorId = teacherId
};

var questions = new List<PaperQuestion>
{
    new PaperQuestion { QuestionId = 1, OrderIndex = 1, Score = 2 },
    new PaperQuestion { QuestionId = 2, OrderIndex = 2, Score = 3 }
};

var createdPaper = await examPaperService.CreateFixedPaperAsync(paper, questions);
```

**1.2 随机试卷（自动抽题）**
```csharp
var config = new RandomPaperConfig
{
    BankId = 1,
    Rules = new List<QuestionSelectionRule>
    {
        new QuestionSelectionRule
        {
            QuestionType = QuestionType.SingleChoice,
            Difficulty = Difficulty.Easy,
            Count = 10,
            ScorePerQuestion = 2
        },
        new QuestionSelectionRule
        {
            QuestionType = QuestionType.MultipleChoice,
            Count = 5,
            ScorePerQuestion = 3
        }
    }
};

var paper = new ExamPaper 
{ 
    Name = "随机测试卷",
    Duration = 90,
    PassScore = 60,
    CreatorId = teacherId
};

var createdPaper = await examPaperService.CreateRandomPaperAsync(paper, config);
// 系统会自动从题库抽取题目，使用Fisher-Yates算法确保随机性
```

**1.3 混合试卷（固定+随机）**
```csharp
var config = new MixedPaperConfig
{
    // 固定题目
    FixedQuestionIds = new List<int> { 1, 2, 3 },
    FixedQuestionScores = new Dictionary<int, decimal>
    {
        { 1, 5 },
        { 2, 5 },
        { 3, 10 }
    },
    // 随机题目
    RandomConfig = new RandomPaperConfig
    {
        BankId = 1,
        Rules = new List<QuestionSelectionRule>
        {
            new QuestionSelectionRule
            {
                QuestionType = QuestionType.SingleChoice,
                Count = 10,
                ScorePerQuestion = 2
            }
        }
    }
};

var createdPaper = await examPaperService.CreateMixedPaperAsync(paper, config);
```

### 2. 考试服务 (ExamService)

#### 完整考试流程

**2.1 开始考试**
```csharp
// 学生开始考试
var examRecord = await examService.StartExamAsync(userId: 3, paperId: 1);
// 系统会检查：
// - 试卷是否激活
// - 考试时间范围
// - 是否已有进行中的考试
```

**2.2 保存答案（实时保存）**
```csharp
// 保存单题答案
await examService.SaveAnswerAsync(
    recordId: examRecord.RecordId,
    questionId: 1,
    answer: "A"
);

// 批量保存答案
var answers = new Dictionary<int, string>
{
    { 1, "A" },
    { 2, "B,C,D" },
    { 3, "True" }
};
await examService.BatchSaveAnswersAsync(examRecord.RecordId, answers);
```

**2.3 查看考试进度**
```csharp
var progress = await examService.GetExamProgressAsync(examRecord.RecordId);
Console.WriteLine($"总题数: {progress.TotalQuestions}");
Console.WriteLine($"已答: {progress.AnsweredQuestions}");
Console.WriteLine($"剩余时间: {progress.RemainingMinutes}分钟");
Console.WriteLine($"是否超时: {progress.IsTimeout}");
```

**2.4 提交考试**
```csharp
var submittedRecord = await examService.SubmitExamAsync(examRecord.RecordId);
// 系统会自动：
// 1. 更新考试状态为"已提交"
// 2. 触发客观题自动评分
// 3. 如果没有主观题，直接计算总分
```

**2.5 超时自动提交（定时任务）**
```csharp
// 在后台定时任务中调用（例如每5分钟执行一次）
await examService.AutoSubmitTimeoutExamsAsync();
// 系统会自动提交所有超时的考试
```

### 3. 评分服务 (GradingService)

#### 自动评分 + 人工评分

**3.1 客观题自动评分**
```csharp
// 考试提交后自动调用
await gradingService.AutoGradeObjectiveQuestionsAsync(recordId);

// 评分规则：
// 单选题：完全正确得分
// 多选题：完全正确满分，选对但不全0.5倍分，有错误0分
// 判断题：True/False精确匹配
// 填空题：模糊匹配（相似度≥85%）
```

**3.2 主观题人工评分**
```csharp
// 教师单个评分
await gradingService.ManualGradeSubjectiveQuestionAsync(
    answerRecordId: 1,
    score: 8,
    comment: "回答正确，思路清晰",
    graderId: teacherId
);

// 教师批量评分
var gradeItems = new List<GradeItem>
{
    new GradeItem { AnswerRecordId = 1, Score = 8, Comment = "很好" },
    new GradeItem { AnswerRecordId = 2, Score = 6, Comment = "基本正确" }
};
await gradingService.BatchGradeSubjectiveQuestionsAsync(gradeItems, teacherId);

// 所有主观题评分完成后，系统会自动计算总分
```

**3.3 查看待评分列表**
```csharp
var pendingRecords = await gradingService.GetPendingGradingRecordsAsync();
foreach (var record in pendingRecords)
{
    Console.WriteLine($"学生: {record.User.RealName}, 试卷: {record.ExamPaper.Name}");
}
```

### 4. 统计服务 (StatisticsService)

#### 多维度数据分析

**4.1 试卷统计**
```csharp
var statistics = await statisticsService.GetPaperStatisticsAsync(paperId: 1);
Console.WriteLine($"考试人数: {statistics.TotalExams}");
Console.WriteLine($"及格率: {statistics.PassRate:F2}%");
Console.WriteLine($"平均分: {statistics.AverageScore:F2}");
Console.WriteLine($"最高分: {statistics.HighestScore}");

// 分数段分布
foreach (var range in statistics.ScoreDistribution)
{
    Console.WriteLine($"{range.Key}: {range.Value}人");
}
// 输出示例：
// 0-60: 5人
// 60-70: 10人
// 70-80: 15人
// 80-90: 12人
// 90-100: 8人
```

**4.2 题目质量分析**
```csharp
var analysis = await statisticsService.GetQuestionAnalysisAsync(
    questionId: 1, 
    paperId: 1
);
Console.WriteLine($"正确率: {analysis.CorrectRate:F2}%");
Console.WriteLine($"得分率: {analysis.ScoreRate:F2}%");
Console.WriteLine($"区分度: {analysis.Discrimination:F2}%");
// 区分度 = 高分组正确率 - 低分组正确率
// 区分度越高，题目质量越好（能区分高分和低分学生）
```

**4.3 成绩排名**
```csharp
var rankings = await statisticsService.GetClassRankingAsync(paperId: 1);
foreach (var ranking in rankings.Take(10))
{
    Console.WriteLine($"#{ranking.Rank} {ranking.RealName}: {ranking.TotalScore}分");
}
```

**4.4 错题统计**
```csharp
var wrongQuestions = await statisticsService.GetWrongQuestionsAsync(userId: 3);
foreach (var wrong in wrongQuestions)
{
    Console.WriteLine($"题目: {wrong.QuestionContent}");
    Console.WriteLine($"错误次数: {wrong.WrongCount}");
    Console.WriteLine($"你的答案: {wrong.UserAnswer}");
    Console.WriteLine($"正确答案: {wrong.CorrectAnswer}");
}
```

## 数据库初始化

### 自动初始化

系统首次运行时会自动：
1. 创建数据库（SQLite）
2. 应用EF Core迁移
3. 初始化种子数据

### 默认账户

| 用户名 | 密码 | 角色 | 说明 |
|--------|------|------|------|
| admin | admin123 | 管理员 | 系统管理员 |
| teacher | teacher123 | 教师 | 示例教师 |
| student | student123 | 学生 | 示例学生 |

### 示例数据

**题库**：通用题库（公开）

**题目**（10道）：
- 2道单选题（难度：简单）
- 2道多选题（难度：中等）
- 2道判断题（难度：简单/中等）
- 2道填空题（难度：简单/中等）
- 2道简答题（难度：中等/困难）

## 如何使用

### 步骤1：调用数据库初始化

在应用启动时调用：

```csharp
// 在App.xaml.cs或Program.cs中
using var context = new ExamSystemDbContext(options);
await DbInitializer.InitializeAsync(context);
```

### 步骤2：注册依赖注入（待实现）

```csharp
// 需要在App.xaml.cs中配置
services.AddDbContext<ExamSystemDbContext>(options =>
    options.UseSqlite(connectionString));

// 注册仓储
services.AddScoped<IExamPaperRepository, ExamPaperRepository>();
services.AddScoped<IExamRecordRepository, ExamRecordRepository>();
// ... 其他仓储

// 注册服务
services.AddScoped<IExamPaperService, ExamPaperService>();
services.AddScoped<IExamService, ExamService>();
services.AddScoped<IGradingService, GradingService>();
services.AddScoped<IStatisticsService, StatisticsService>();
```

### 步骤3：在ViewModel中使用服务

```csharp
public class ExamViewModel : ObservableObject
{
    private readonly IExamService _examService;
    private readonly IGradingService _gradingService;

    public ExamViewModel(
        IExamService examService,
        IGradingService gradingService)
    {
        _examService = examService;
        _gradingService = gradingService;
    }

    public async Task StartExam()
    {
        var record = await _examService.StartExamAsync(userId, paperId);
        // 更新UI
    }
}
```

## 关键技术点

### 1. Fisher-Yates洗牌算法

用于随机组卷，确保题目随机分布：

```csharp
private List<T> FisherYatesShuffle<T>(List<T> list, Random random)
{
    var shuffled = new List<T>(list);
    int n = shuffled.Count;
    
    while (n > 1)
    {
        n--;
        int k = random.Next(n + 1);
        var temp = shuffled[k];
        shuffled[k] = shuffled[n];
        shuffled[n] = temp;
    }
    
    return shuffled;
}
```

时间复杂度：O(n)，空间复杂度：O(n)

### 2. 填空题模糊匹配

使用Levenshtein距离算法，相似度≥85%即判定为正确：

```csharp
public static bool FuzzyMatch(string userAnswer, string correctAnswer)
{
    var similarity = CalculateSimilarity(userAnswer, correctAnswer);
    return similarity >= 0.85m;
}
```

支持多个标准答案（用竖线分隔）：
- 答案："ViewModel|视图模型"
- 学生答："viewmodel" → 正确（忽略大小写）
- 学生答："视图模型" → 正确

### 3. 多选题评分策略

```csharp
private void GradeMultipleChoice(AnswerRecord answer, Question question, decimal score)
{
    var result = AnswerComparer.CompareMultipleChoice(
        answer.UserAnswer, 
        question.Answer
    );
    
    if (result == 1.0m)
        answer.Score = score;           // 完全正确：满分
    else if (result > 0)
        answer.Score = score * 0.5m;    // 部分正确：0.5倍分
    else
        answer.Score = 0;               // 有错误：0分
}
```

### 4. 区分度计算

评估题目质量的重要指标：

```csharp
// 1. 按总分排序，取前1/3为高分组，后1/3为低分组
var topCount = totalRecords.Count / 3;
var topRecords = totalRecords.Take(topCount);
var bottomRecords = totalRecords.TakeLast(topCount);

// 2. 分别计算正确率
var topCorrectRate = topAnswers.Count(a => a.IsCorrect == true) / topAnswers.Count * 100;
var bottomCorrectRate = bottomAnswers.Count(a => a.IsCorrect == true) / bottomAnswers.Count * 100;

// 3. 计算区分度
var discrimination = topCorrectRate - bottomCorrectRate;

// 解读：
// 区分度 > 40%：题目质量优秀
// 区分度 30%-40%：题目质量良好
// 区分度 20%-30%：题目质量一般
// 区分度 < 20%：题目质量较差，需要修改
```

## 下一步开发建议

### 优先级1（紧急）
1. 配置App.xaml.cs依赖注入
2. 创建MainWindow主窗口
3. 实现LoginViewModel和LoginWindow

### 优先级2（重要）
1. 实现QuestionManagementViewModel（题目管理）
2. 实现ExamPaperViewModel（组卷）
3. 实现ExamViewModel（考试）

### 优先级3（一般）
1. 实现GradingViewModel（评分）
2. 实现StatisticsViewModel（统计）
3. 编写单元测试

## 常见问题

### Q1: 如何修改数据库连接字符串？
A: 编辑 `ExamSystem.UI/appsettings.json` 文件中的 `ConnectionStrings:DefaultConnection`

### Q2: 如何添加新的默认用户？
A: 编辑 `ExamSystem.Repository/Context/DbInitializer.cs` 中的 `SeedDataAsync` 方法

### Q3: 如何自定义评分规则？
A: 修改 `ExamSystem.Services/Implementations/GradingService.cs` 中的评分方法

### Q4: 如何调整填空题匹配阈值？
A: 修改 `ExamSystem.Infrastructure/Utils/AnswerComparer.cs` 中的 `FuzzyMatch` 方法，默认阈值为0.85

### Q5: 如何查看数据库内容？
A: 使用SQLite工具（如DB Browser for SQLite）打开 `exam_system.db` 文件

## 技术支持

- 设计文档：`DEVELOPMENT_GUIDE.md`
- 项目结构：`PROJECT_STRUCTURE.md`
- 实施总结：`IMPLEMENTATION_SUMMARY_FINAL.md`
- 进度报告：`IMPLEMENTATION_PROGRESS.md`

---

**文档版本**: v1.0  
**最后更新**: 2025-10-15  
**适用版本**: 核心后端完成版
