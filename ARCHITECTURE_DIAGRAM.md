# 解耦后的项目架构图

## 整体架构

```
┌─────────────────────────────────────────────────────────────┐
│                    表现层 (Presentation)                     │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │            ExamSystem.UI (WPF)                       │  │
│  │  - Views (XAML)                                      │  │
│  │  - Services Implementation                           │  │
│  │    • NavigationService                               │  │
│  │    • DialogService                                   │  │
│  │    • NotificationService                             │  │
│  │    • FileDialogService                               │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                             ↑
                             │ 实现
                             │
┌─────────────────────────────────────────────────────────────┐
│                    抽象层 (Abstractions)                     │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │      ExamSystem.Abstractions                         │  │
│  │  Services/                                           │  │
│  │    • INavigationService                              │  │
│  │    • IDialogService                                  │  │
│  │    • INotificationService                            │  │
│  │    • IFileDialogService                              │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                             ↑
                             │ 引用
                             │
┌─────────────────────────────────────────────────────────────┐
│                 视图模型层 (ViewModels)                      │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │      ExamSystem.ViewModels                           │  │
│  │  - LoginViewModel                                    │  │
│  │  - MainViewModel                                     │  │
│  │  - QuestionBankViewModel                             │  │
│  │  - ExamPaperViewModel                                │  │
│  │  - ExamTakingViewModel                               │  │
│  │  - UserManagementViewModel                           │  │
│  │  - SystemSettingsViewModel                           │  │
│  │  - StatisticsViewModel                               │  │
│  │  - ScoreViewModel                                    │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                             ↑
                             │ 引用
                             │
┌─────────────────────────────────────────────────────────────┐
│                  业务逻辑层 (Business)                       │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │      ExamSystem.Services                             │  │
│  │  - UserService                                       │  │
│  │  - QuestionService                                   │  │
│  │  - ExamPaperService                                  │  │
│  │  - ExamService                                       │  │
│  │  - GradingService                                    │  │
│  │  - StatisticsService                                 │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                             ↑
                             │ 引用
                             │
┌─────────────────────────────────────────────────────────────┐
│                 数据访问层 (Data Access)                     │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │      ExamSystem.Repository                           │  │
│  │  - UserRepository                                    │  │
│  │  - QuestionRepository                                │  │
│  │  - QuestionBankRepository                            │  │
│  │  - ExamPaperRepository                               │  │
│  │  - ExamRecordRepository                              │  │
│  │  - AnswerRecordRepository                            │  │
│  │  - ExamSystemDbContext                               │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                             ↑
                             │ 引用
                             │
┌─────────────────────────────────────────────────────────────┐
│                   领域层 (Domain)                            │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │      ExamSystem.Domain                               │  │
│  │  Entities/                                           │  │
│  │    • User, Question, QuestionBank                    │  │
│  │    • ExamPaper, ExamRecord, AnswerRecord             │  │
│  │  Enums/                                              │  │
│  │    • UserRole, QuestionType, Difficulty              │  │
│  │    • PaperType, PaperStatus, ExamStatus              │  │
│  │  DTOs/                                               │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                             ↑
                             │
┌─────────────────────────────────────────────────────────────┐
│                 基础设施层 (Infrastructure)                  │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │      ExamSystem.Infrastructure                       │  │
│  │  - PasswordHelper                                    │  │
│  │  - JsonHelper                                        │  │
│  │  - AnswerComparer                                    │  │
│  │  - SystemConfig                                      │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

## 关键改进

### 解耦前

```
ViewModels ←──────→ UI
   (循环依赖)
```

### 解耦后

```
         Abstractions (接口契约)
              ↑     ↑
              │     │
   ViewModels │     │ UI
   (使用接口) │     │ (实现接口)
              │     │
              └──DI──┘
          (依赖注入连接)
```

## 依赖方向规则

1. **单向依赖**: 所有依赖箭头向下（或向抽象层）
2. **依赖倒置**: ViewModels 和 UI 都依赖 Abstractions
3. **无循环**: 任何两个模块之间不存在循环依赖

## 核心优势

✅ **解耦**: ViewModels 与 UI 技术完全解耦  
✅ **可测试**: ViewModels 可独立进行单元测试  
✅ **可复用**: ViewModels 可用于不同 UI 框架  
✅ **可维护**: 清晰的分层和职责分离  
✅ **可扩展**: 易于添加新功能和新实现
