# ExamSystem.UI - 在线考试系统 UI层实现文档

## 项目概述

本项目是基于WPF的在线考试系统桌面应用程序，采用MVVM架构模式，使用Material Design风格界面。

## 已实现的功能

### 1. 核心服务层 (Services/)

#### 导航服务 (INavigationService / NavigationService)
- 页面导航管理
- ViewModel切换
- 导航历史栈
- 支持带参数导航

#### 对话框服务 (IDialogService / DialogService)
- 消息对话框
- 确认对话框
- 错误对话框
- 输入对话框
- 自定义对话框

#### 通知服务 (INotificationService / NotificationService)
- 成功提示
- 警告提示
- 错误提示
- 信息提示

#### 文件对话框服务 (IFileDialogService / FileDialogService)
- 打开文件对话框
- 保存文件对话框
- 文件夹选择对话框

### 2. 资源文件 (Resources/)

#### 颜色资源 (Colors.xaml)
- 主色调：Primary, Secondary
- 状态色：Success, Warning, Error, Info
- 中性色：Background, Surface, Border, Text等

#### 字体资源 (Fonts.xaml)
- 字体大小：XSmall ~ XXLarge
- 字重：Light, Regular, Medium, Bold

#### 按钮样式 (ButtonStyles.xaml)
- PrimaryButton：主按钮
- SecondaryButton：次要按钮
- IconButton：图标按钮

#### 输入框样式 (TextBoxStyles.xaml)
- StandardTextBox：标准输入框
- StandardPasswordBox：密码框

#### 卡片样式 (CardStyles.xaml)
- CardBorder：卡片容器
- StatisticCard：统计卡片

### 3. 自定义控件 (Controls/)

#### QuestionDisplay
- 题目显示控件
- 支持多种题型（单选、多选、判断、填空、主观）
- 可配置是否显示答案
- 支持只读模式

#### PaginationControl
- 分页控件
- 支持首页、上一页、下一页、末页导航
- 显示当前页/总页数
- 页码改变事件

#### CountdownTimer
- 倒计时控件
- 自动倒计时
- 警告阈值提示
- 时间到达事件
- 格式化时间显示

### 4. 转换器 (Converters/)

- BooleanToVisibilityConverter：布尔值转可见性
- InverseBooleanConverter：布尔值取反
- StringToVisibilityConverter：字符串转可见性
- NullToVisibilityConverter：空对象转可见性

### 5. 视图 (Views/)

#### LoginWindow
- 用户登录窗口
- 用户名/密码输入
- 记住密码选项
- 加载指示器
- 错误提示

#### MainWindow
- 主窗口框架
- 顶部标题栏
- 左侧导航菜单（可展开/收起）
- 右侧内容区域
- 根据用户角色动态加载菜单

#### HomeView
- 首页视图
- 系统概览
- 统计卡片（用户数、题库数、试卷数、考试数）

### 6. ViewModels

#### LoginViewModel
- 用户登录逻辑
- 输入验证
- 登录命令
- 错误处理

#### MainViewModel
- 主窗口管理
- 当前用户信息
- 导航管理
- 菜单展开/收起
- 退出登录

#### HomeViewModel
- 首页数据展示
- 统计数据加载

#### UserManagementViewModel
- 用户列表管理
- 用户搜索
- 角色筛选
- 分页功能
- 删除用户

### 7. 应用程序入口 (App.xaml / App.xaml.cs)

#### 依赖注入配置
- DbContext注册
- Repository注册
- Service注册
- ViewModel注册
- UI服务注册

#### 日志配置
- Serilog日志记录
- 文件日志输出
- 控制台日志输出

#### 数据库初始化
- 自动创建数据库
- 种子数据初始化

#### 全局异常处理
- 未处理异常捕获
- 错误日志记录
- 友好错误提示

## 项目依赖

### NuGet包
- MaterialDesignThemes (4.9.0)
- MaterialDesignColors (2.1.4)
- LiveCharts.Wpf (0.9.7)
- CommunityToolkit.Mvvm (8.2.2)
- Microsoft.Extensions.DependencyInjection (8.0.0)
- Microsoft.Extensions.Configuration (8.0.0)
- Microsoft.Extensions.Configuration.Json (8.0.0)
- Serilog (3.1.1)
- Serilog.Sinks.File (5.0.0)
- Serilog.Sinks.Console (5.0.1)
- System.Windows.Forms (4.0.0)

### 项目引用
- ExamSystem.ViewModels
- ExamSystem.Services
- ExamSystem.Infrastructure

## 待完成的功能

### 管理员页面
- 用户管理详细页面
- 系统设置页面

### 教师/管理员共用页面
- 题库管理页面
- 题目编辑对话框
- 试卷管理页面
- 组卷向导
- 考试监控页面
- 评分管理页面
- 统计分析页面

### 学生页面
- 考试列表页面
- 答题页面
- 成绩查看页面
- 错题本页面

### 其他ViewModel
- QuestionBankViewModel
- QuestionEditorViewModel
- ExamPaperViewModel
- PaperCreationWizardViewModel
- ExamMonitorViewModel
- GradingViewModel
- StatisticsViewModel
- ExamListViewModel
- ExamTakingViewModel
- ScoreViewModel
- WrongQuestionViewModel
- SystemSettingsViewModel

## 使用说明

### 运行项目

1. 确保安装了 .NET 5.0 SDK
2. 还原NuGet包：
   ```bash
   dotnet restore
   ```
3. 编译项目：
   ```bash
   dotnet build
   ```
4. 运行项目：
   ```bash
   dotnet run --project ExamSystem.UI
   ```

### 配置文件

appsettings.json 配置项：
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=exam.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

## 设计模式

### MVVM模式
- View：XAML定义UI布局
- ViewModel：继承ObservableObject，处理业务逻辑
- Model：领域实体和DTO

### 依赖注入
- 通过IServiceProvider管理对象生命周期
- Singleton：全局单例服务
- Scoped：请求范围服务
- Transient：每次创建新实例

### 服务定位
- 导航服务：页面跳转
- 对话框服务：弹窗管理
- 通知服务：消息提示

## 扩展指南

### 添加新页面

1. 在Views文件夹创建UserControl
2. 创建对应的ViewModel
3. 在App.xaml.cs注册ViewModel
4. 在MainWindow.xaml.cs添加菜单项
5. 实现导航逻辑

### 添加新样式

1. 在Resources文件夹创建新的资源字典
2. 在App.xaml中引用
3. 在视图中使用StaticResource引用

## 注意事项

1. **PasswordBox绑定**：PasswordBox的Password属性不支持绑定，需要在代码后台处理
2. **异步命令**：使用AsyncRelayCommand处理异步操作
3. **UI线程**：数据访问操作在后台线程执行，UI更新必须在主线程
4. **资源释放**：使用using语句确保资源正确释放
5. **异常处理**：所有服务调用需要try-catch处理异常

## 开发团队

- 架构设计：基于设计文档
- UI实现：WPF + Material Design
- 数据访问：Entity Framework Core + SQLite

## 版本历史

### v1.0.0 (当前)
- 实现核心服务层
- 实现主题和样式资源
- 实现基础自定义控件
- 实现登录、主窗口、首页
- 配置依赖注入和日志系统
