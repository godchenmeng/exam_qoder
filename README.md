# 在线考试系统 (ExamSystem)

## 项目简介

基于 WPF 和 .NET 5.0 的桌面在线考试系统,支持题库管理、智能组卷、在线考试、自动评分、成绩统计等功能。

## 技术栈

- **开发框架**: WPF + .NET 5.0
- **架构模式**: MVVM (Model-View-ViewModel)
- **数据库**: SQLite
- **ORM框架**: Entity Framework Core
- **依赖注入**: Microsoft.Extensions.DependencyInjection
- **日志框架**: Serilog
- **MVVM框架**: CommunityToolkit.Mvvm
- **测试框架**: xUnit

## 项目结构

```
ExamSystem/
├── ExamSystem.Domain/              # 领域模型层
│   ├── Entities/                   # 实体类
│   ├── Enums/                      # 枚举
│   └── DTOs/                       # 数据传输对象
├── ExamSystem.Infrastructure/      # 基础设施层
│   ├── Common/                     # 公共组件
│   └── Utils/                      # 工具类
├── ExamSystem.Repository/          # 数据访问层
│   ├── Context/                    # 数据库上下文
│   ├── Repositories/               # 仓储实现
│   └── Interfaces/                 # 仓储接口
├── ExamSystem.Services/            # 业务服务层
│   ├── Interfaces/                 # 服务接口
│   └── Implementations/            # 服务实现
├── ExamSystem.ViewModels/          # 视图模型层
│   └── ...                         # 各页面ViewModel
├── ExamSystem.UI/                  # UI层
│   ├── Views/                      # XAML视图
│   ├── Controls/                   # 自定义控件
│   ├── Resources/                  # 资源文件
│   └── Themes/                     # 主题样式
└── ExamSystem.Tests/               # 测试项目
    ├── UnitTests/                  # 单元测试
    └── IntegrationTests/           # 集成测试
```

## 核心功能

### 1. 题库管理
- 题库创建、编辑、删除
- 题目管理(单选、多选、判断、填空、主观题)
- 题目导入导出(Excel、JSON)
- 题目搜索和筛选

### 2. 组卷管理
- 固定组卷
- 随机组卷(按题型、难度、标签抽题)
- 混合组卷
- 试卷预览和导出

### 3. 考试管理
- 考试流程控制
- 实时答题保存
- 考试计时和提醒
- 防作弊监控

### 4. 评分管理
- 客观题自动评分
- 主观题人工评分
- 成绩计算和发布

### 5. 统计分析
- 考试数据统计
- 题目质量分析
- 成绩报表生成
- 数据可视化

### 6. 用户管理
- 用户认证和授权
- 角色权限管理(管理员/教师/学生)
- 用户批量导入

## 数据模型

### 核心实体

- **Users**: 用户表
- **QuestionBanks**: 题库表
- **Questions**: 题目表
- **Options**: 选项表
- **ExamPapers**: 试卷表
- **PaperQuestions**: 试卷题目关联表
- **ExamRecords**: 考试记录表
- **AnswerRecords**: 答题记录表

## 快速开始

### 环境要求

- .NET 5.0 SDK 或更高版本
- Visual Studio 2019+ 或 JetBrains Rider

### 构建项目

```bash
# 还原依赖
dotnet restore

# 编译解决方案
dotnet build

# 运行测试
dotnet test

# 运行应用
dotnet run --project ExamSystem.UI
```

### 数据库初始化

首次运行时,系统会自动创建 SQLite 数据库并执行迁移。

## 开发指南

### 编码规范

- 遵循 C# 编码规范
- 使用 async/await 进行异步编程
- 所有公共方法需添加 XML 注释
- 服务层方法需进行异常处理

### MVVM 模式

- View 不包含业务逻辑
- ViewModel 使用 CommunityToolkit.Mvvm 的特性
- 通过依赖注入管理 ViewModel 和 Service

### 数据库操作

- 使用 Repository 模式进行数据访问
- 关键业务操作使用事务
- 使用 EF Core 的 Migration 管理数据库结构变更

## 测试

### 单元测试

使用 xUnit 和 Moq 进行单元测试,覆盖:
- Repository 层
- Service 层
- ViewModel 层

### 集成测试

测试完整业务流程:
- 题目管理流程
- 组卷流程
- 考试流程
- 评分流程

## 许可证

本项目仅供学习和研究使用。

## 作者

Qoder AI Assistant

## 更新日志

### v1.0.0 (2025-10-15)
- 初始版本
- 实现核心功能模块
