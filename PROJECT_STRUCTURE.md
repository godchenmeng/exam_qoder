# 在线考试系统 - 项目文件结构

## 完整项目树

```
exam_qoder/
├── .git/                                # Git版本控制
├── .gitignore                           # Git忽略文件
├── README.md                            # 项目说明文档
├── IMPLEMENTATION_SUMMARY.md            # 实现总结文档
├── DEVELOPMENT_GUIDE.md                 # 开发指南文档
├── ExamSystem.sln                       # 解决方案文件
│
├── ExamSystem.Domain/                   # 领域模型层 ✅
│   ├── Enums/                          # 枚举类型
│   │   ├── QuestionType.cs             # 题型枚举
│   │   ├── Difficulty.cs               # 难度枚举
│   │   ├── UserRole.cs                 # 用户角色枚举
│   │   ├── PaperType.cs                # 试卷类型枚举
│   │   ├── ExamStatus.cs               # 考试状态枚举
│   │   └── PaperStatus.cs              # 试卷状态枚举
│   ├── Entities/                       # 实体类
│   │   ├── User.cs                     # 用户实体
│   │   ├── QuestionBank.cs             # 题库实体
│   │   ├── Question.cs                 # 题目实体
│   │   ├── Option.cs                   # 选项实体
│   │   ├── ExamPaper.cs                # 试卷实体
│   │   ├── PaperQuestion.cs            # 试卷题目关联实体
│   │   ├── ExamRecord.cs               # 考试记录实体
│   │   └── AnswerRecord.cs             # 答题记录实体
│   ├── DTOs/                           # 数据传输对象
│   │   ├── UserLoginResult.cs          # 登录结果DTO
│   │   ├── ValidationResult.cs         # 验证结果DTO
│   │   └── PagedResult.cs              # 分页结果DTO
│   └── ExamSystem.Domain.csproj        # 项目文件
│
├── ExamSystem.Infrastructure/           # 基础设施层 ✅
│   ├── Common/                         # 公共组件
│   │   ├── SystemConfig.cs             # 系统配置类
│   │   └── Constants.cs                # 常量定义类
│   ├── Utils/                          # 工具类
│   │   ├── PasswordHelper.cs           # 密码加密工具
│   │   ├── JsonHelper.cs               # JSON序列化工具
│   │   └── AnswerComparer.cs           # 答案比较工具
│   └── ExamSystem.Infrastructure.csproj # 项目文件
│
├── ExamSystem.Repository/               # 数据访问层 ✅ (部分完成)
│   ├── Context/                        # 数据库上下文
│   │   └── ExamSystemDbContext.cs      # EF Core上下文
│   ├── Interfaces/                     # 仓储接口 (待实现)
│   │   ├── IRepository.cs              # 通用仓储接口
│   │   ├── IUserRepository.cs          # 用户仓储接口
│   │   ├── IQuestionRepository.cs      # 题目仓储接口
│   │   └── ...                         # 其他仓储接口
│   ├── Repositories/                   # 仓储实现 (待实现)
│   │   ├── Repository.cs               # 通用仓储实现
│   │   ├── UserRepository.cs           # 用户仓储实现
│   │   ├── QuestionRepository.cs       # 题目仓储实现
│   │   └── ...                         # 其他仓储实现
│   └── ExamSystem.Repository.csproj    # 项目文件
│
├── ExamSystem.Services/                 # 业务服务层 (待实现)
│   ├── Interfaces/                     # 服务接口
│   │   ├── IUserService.cs             # 用户服务接口
│   │   ├── IQuestionService.cs         # 题目服务接口
│   │   ├── IExamPaperService.cs        # 试卷服务接口
│   │   ├── IExamService.cs             # 考试服务接口
│   │   ├── IGradingService.cs          # 评分服务接口
│   │   └── IStatisticsService.cs       # 统计服务接口
│   ├── Implementations/                # 服务实现
│   │   ├── UserService.cs              # 用户服务实现
│   │   ├── QuestionService.cs          # 题目服务实现
│   │   ├── ExamPaperService.cs         # 试卷服务实现
│   │   ├── ExamService.cs              # 考试服务实现
│   │   ├── GradingService.cs           # 评分服务实现
│   │   └── StatisticsService.cs        # 统计服务实现
│   └── ExamSystem.Services.csproj      # 项目文件
│
├── ExamSystem.ViewModels/               # 视图模型层 (待实现)
│   ├── LoginViewModel.cs               # 登录视图模型
│   ├── MainViewModel.cs                # 主窗口视图模型
│   ├── QuestionBankViewModel.cs        # 题库管理视图模型
│   ├── QuestionManagementViewModel.cs  # 题目管理视图模型
│   ├── ExamPaperViewModel.cs           # 试卷管理视图模型
│   ├── ExamViewModel.cs                # 考试视图模型
│   ├── GradingViewModel.cs             # 评分视图模型
│   ├── StatisticsViewModel.cs          # 统计视图模型
│   └── ExamSystem.ViewModels.csproj    # 项目文件
│
├── ExamSystem.UI/                       # UI层 (待实现)
│   ├── App.xaml                        # 应用程序
│   ├── App.xaml.cs                     # 应用程序代码
│   ├── Views/                          # 视图页面
│   │   ├── LoginWindow.xaml            # 登录窗口
│   │   ├── MainWindow.xaml             # 主窗口
│   │   ├── QuestionBankView.xaml       # 题库管理页面
│   │   ├── QuestionManagementView.xaml # 题目管理页面
│   │   ├── ExamPaperView.xaml          # 试卷管理页面
│   │   ├── ExamView.xaml               # 考试页面
│   │   ├── GradingView.xaml            # 评分页面
│   │   └── StatisticsView.xaml         # 统计页面
│   ├── Controls/                       # 自定义控件
│   │   ├── PaginationControl.xaml      # 分页控件
│   │   ├── QuestionViewer.xaml         # 题目展示控件
│   │   └── AnswerInput.xaml            # 答题输入控件
│   ├── Resources/                      # 资源文件
│   │   ├── Styles/                     # 样式
│   │   ├── Images/                     # 图片
│   │   └── Converters/                 # 值转换器
│   ├── Themes/                         # 主题
│   │   ├── LightTheme.xaml             # 亮色主题
│   │   └── DarkTheme.xaml              # 暗色主题
│   └── ExamSystem.UI.csproj            # 项目文件
│
└── ExamSystem.Tests/                    # 测试项目 (待实现)
    ├── UnitTests/                      # 单元测试
    │   ├── Infrastructure/             # 基础设施层测试
    │   │   ├── PasswordHelperTests.cs  # 密码工具测试
    │   │   └── AnswerComparerTests.cs  # 答案比较测试
    │   ├── Services/                   # 服务层测试
    │   │   ├── UserServiceTests.cs     # 用户服务测试
    │   │   └── GradingServiceTests.cs  # 评分服务测试
    │   └── ViewModels/                 # 视图模型测试
    │       └── LoginViewModelTests.cs  # 登录视图模型测试
    ├── IntegrationTests/               # 集成测试
    │   ├── ExamFlowTests.cs            # 考试流程测试
    │   └── PaperCreationTests.cs       # 组卷流程测试
    └── ExamSystem.Tests.csproj         # 项目文件
```

## 项目文件统计

### 已完成文件 (✅)

| 层级 | 文件数 | 状态 |
|------|--------|------|
| Domain - Enums | 6 | ✅ 完成 |
| Domain - Entities | 8 | ✅ 完成 |
| Domain - DTOs | 3 | ✅ 完成 |
| Infrastructure - Utils | 3 | ✅ 完成 |
| Infrastructure - Common | 2 | ✅ 完成 |
| Repository - Context | 1 | ✅ 完成 |

**总计: 23个核心文件已完成**

### 待实现文件 (📝)

| 层级 | 预计文件数 | 优先级 |
|------|-----------|--------|
| Repository - Interfaces | 6 | 高 |
| Repository - Implementations | 6 | 高 |
| Services - Interfaces | 6 | 高 |
| Services - Implementations | 6 | 高 |
| ViewModels | 8 | 中 |
| UI - Views | 8 | 中 |
| UI - Controls | 3 | 低 |
| UI - Resources | 多个 | 低 |
| Tests | 10+ | 中 |

**预计待实现: 50+个文件**

## 核心文件说明

### 领域模型层

#### Enums (枚举)
所有枚举都包含完整的XML注释,定义了系统中使用的所有枚举类型。

#### Entities (实体)
所有实体类都包含:
- 主键属性
- 外键属性
- 导航属性
- 完整的XML注释

#### DTOs
提供了核心的数据传输对象,用于层与层之间的数据传递。

### 基础设施层

#### PasswordHelper
- 使用SHA256算法进行密码加密
- 提供密码验证功能
- 支持随机密码生成
- 密码强度验证

#### JsonHelper
- 基于System.Text.Json
- 提供序列化和反序列化
- 包含异常处理

#### AnswerComparer
- 精确匹配算法
- 模糊匹配(Levenshtein距离)
- 多选题评分逻辑
- 相似度计算

### 数据访问层

#### ExamSystemDbContext
- 配置了所有8个实体
- 定义了实体关系
- 配置了索引
- 包含种子数据(默认管理员)

## NuGet包依赖

| 包名 | 版本 | 用途 |
|------|------|------|
| Microsoft.EntityFrameworkCore.Sqlite | 5.0.17 | SQLite数据库支持 |
| Microsoft.NET.Test.Sdk | - | 测试SDK |
| xUnit | - | 测试框架 |

## 编译状态

✅ **所有项目编译成功,0警告,0错误**

```
ExamSystem.Domain -> bin/Debug/net5.0/ExamSystem.Domain.dll
ExamSystem.Infrastructure -> bin/Debug/net5.0/ExamSystem.Infrastructure.dll
ExamSystem.Repository -> bin/Debug/net5.0/ExamSystem.Repository.dll
ExamSystem.Services -> bin/Debug/net5.0/ExamSystem.Services.dll
ExamSystem.ViewModels -> bin/Debug/net5.0/ExamSystem.ViewModels.dll
ExamSystem.UI -> bin/Debug/net5.0/ExamSystem.UI.dll
ExamSystem.Tests -> bin/Debug/net5.0/ExamSystem.Tests.dll
```

## 下一步开发重点

### 优先级1 - 数据访问层完善
1. 创建通用Repository接口和实现
2. 创建各实体的专用Repository
3. 添加EF Core迁移

### 优先级2 - 服务层实现
1. 实现UserService(用户认证、管理)
2. 实现QuestionService(题目CRUD)
3. 实现ExamPaperService(组卷逻辑)
4. 实现ExamService(考试流程)
5. 实现GradingService(评分逻辑)

### 优先级3 - UI层实现
1. 配置WPF应用
2. 实现登录界面
3. 实现主窗口框架
4. 逐步实现各功能页面

## 代码质量

- ✅ 所有公共类都有XML注释
- ✅ 遵循C#命名规范
- ✅ 使用了异步编程模式(准备)
- ✅ 实体关系配置完整
- ✅ 包含种子数据

## 项目规模估算

| 指标 | 数量 |
|------|------|
| 代码行数(已完成) | ~2000行 |
| 预计总代码行数 | ~8000-10000行 |
| 完成度 | 约20-25% |

## 备注

本项目采用严格的分层架构,各层职责清晰,便于维护和扩展。当前已完成核心基础层的实现,为后续业务逻辑开发奠定了坚实基础。
