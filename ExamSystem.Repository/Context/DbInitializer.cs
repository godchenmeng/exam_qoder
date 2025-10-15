using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Enums;
using ExamSystem.Infrastructure.Utils;

namespace ExamSystem.Repository.Context
{
    /// <summary>
    /// 数据库初始化帮助类
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// 初始化数据库
        /// </summary>
        public static async Task InitializeAsync(ExamSystemDbContext context)
        {
            try
            {
                // 确保数据库已创建
                await context.Database.EnsureCreatedAsync();

                // 应用待处理的迁移
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    await context.Database.MigrateAsync();
                }

                // 初始化种子数据
                await SeedDataAsync(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"数据库初始化失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 种子数据初始化
        /// </summary>
        private static async Task SeedDataAsync(ExamSystemDbContext context)
        {
            // 1. 创建默认管理员账户
            if (!await context.Users.AnyAsync(u => u.Username == "admin"))
            {
                var adminUser = new User
                {
                    Username = "admin",
                    PasswordHash = PasswordHelper.HashPassword("admin123"),
                    RealName = "系统管理员",
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
            }

            // 2. 创建示例教师账户
            if (!await context.Users.AnyAsync(u => u.Username == "teacher"))
            {
                var teacherUser = new User
                {
                    Username = "teacher",
                    PasswordHash = PasswordHelper.HashPassword("teacher123"),
                    RealName = "示例教师",
                    Role = UserRole.Teacher,
                    Email = "teacher@example.com",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                context.Users.Add(teacherUser);
                await context.SaveChangesAsync();
            }

            // 3. 创建示例学生账户
            if (!await context.Users.AnyAsync(u => u.Username == "student"))
            {
                var studentUser = new User
                {
                    Username = "student",
                    PasswordHash = PasswordHelper.HashPassword("student123"),
                    RealName = "示例学生",
                    Role = UserRole.Student,
                    Email = "student@example.com",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                context.Users.Add(studentUser);
                await context.SaveChangesAsync();
            }

            // 4. 创建示例题库
            var adminId = (await context.Users.FirstOrDefaultAsync(u => u.Username == "admin"))?.UserId ?? 1;
            
            if (!await context.QuestionBanks.AnyAsync(b => b.Name == "通用题库"))
            {
                var questionBank = new QuestionBank
                {
                    Name = "通用题库",
                    Description = "系统默认题库，包含各类型示例题目",
                    Category = "通用",
                    CreatorId = adminId,
                    IsPublic = true,
                    CreatedAt = DateTime.Now
                };
                context.QuestionBanks.Add(questionBank);
                await context.SaveChangesAsync();

                // 5. 创建示例题目
                await CreateSampleQuestionsAsync(context, questionBank.BankId);
            }
        }

        /// <summary>
        /// 创建示例题目
        /// </summary>
        private static async Task CreateSampleQuestionsAsync(ExamSystemDbContext context, int bankId)
        {
            var questions = new[]
            {
                // 单选题
                new Question
                {
                    BankId = bankId,
                    QuestionType = QuestionType.SingleChoice,
                    Content = "以下哪个不是面向对象编程的特性？",
                    Answer = "A",
                    DefaultScore = 2,
                    Difficulty = Difficulty.Easy,
                    Explanation = "面向对象编程的三大特性是封装、继承和多态",
                    CreatedAt = DateTime.Now
                },
                new Question
                {
                    BankId = bankId,
                    QuestionType = QuestionType.SingleChoice,
                    Content = "C#中，string类型是：",
                    Answer = "B",
                    DefaultScore = 2,
                    Difficulty = Difficulty.Easy,
                    Explanation = "string是引用类型",
                    CreatedAt = DateTime.Now
                },
                // 多选题
                new Question
                {
                    BankId = bankId,
                    QuestionType = QuestionType.MultipleChoice,
                    Content = "以下哪些是C#的访问修饰符？",
                    Answer = "A,B,C,D",
                    DefaultScore = 3,
                    Difficulty = Difficulty.Medium,
                    Explanation = "C#的访问修饰符包括public、private、protected、internal等",
                    CreatedAt = DateTime.Now
                },
                new Question
                {
                    BankId = bankId,
                    QuestionType = QuestionType.MultipleChoice,
                    Content = "LINQ查询可以用于以下哪些数据源？",
                    Answer = "A,B,C",
                    DefaultScore = 3,
                    Difficulty = Difficulty.Medium,
                    Explanation = "LINQ可以查询数组、集合、数据库等多种数据源",
                    CreatedAt = DateTime.Now
                },
                // 判断题
                new Question
                {
                    BankId = bankId,
                    QuestionType = QuestionType.TrueFalse,
                    Content = "接口可以包含字段成员。",
                    Answer = "False",
                    DefaultScore = 1,
                    Difficulty = Difficulty.Easy,
                    Explanation = "接口只能包含方法、属性、事件和索引器，不能包含字段",
                    CreatedAt = DateTime.Now
                },
                new Question
                {
                    BankId = bankId,
                    QuestionType = QuestionType.TrueFalse,
                    Content = "C#中的值类型存储在栈上，引用类型存储在堆上。",
                    Answer = "True",
                    DefaultScore = 1,
                    Difficulty = Difficulty.Medium,
                    Explanation = "值类型通常存储在栈上，引用类型的实例存储在堆上",
                    CreatedAt = DateTime.Now
                },
                // 填空题
                new Question
                {
                    BankId = bankId,
                    QuestionType = QuestionType.FillBlank,
                    Content = "在C#中，用于定义泛型类的关键字是______。",
                    Answer = "<>|泛型|generic",
                    DefaultScore = 2,
                    Difficulty = Difficulty.Easy,
                    Explanation = "使用尖括号<>定义泛型",
                    CreatedAt = DateTime.Now
                },
                new Question
                {
                    BankId = bankId,
                    QuestionType = QuestionType.FillBlank,
                    Content = "MVVM模式中的VM代表______。",
                    Answer = "ViewModel|视图模型",
                    DefaultScore = 2,
                    Difficulty = Difficulty.Medium,
                    Explanation = "MVVM中VM是ViewModel（视图模型）",
                    CreatedAt = DateTime.Now
                },
                // 简答题
                new Question
                {
                    BankId = bankId,
                    QuestionType = QuestionType.ShortAnswer,
                    Content = "请简述C#中委托（Delegate）的作用。",
                    Answer = "委托是一种引用类型，用于封装方法，可以实现回调函数、事件处理等功能。",
                    DefaultScore = 5,
                    Difficulty = Difficulty.Medium,
                    Explanation = "委托是类型安全的函数指针",
                    CreatedAt = DateTime.Now
                },
                new Question
                {
                    BankId = bankId,
                    QuestionType = QuestionType.ShortAnswer,
                    Content = "请解释async和await关键字的作用。",
                    Answer = "async和await用于异步编程，async标记异步方法，await等待异步操作完成。",
                    DefaultScore = 5,
                    Difficulty = Difficulty.Hard,
                    Explanation = "这两个关键字简化了异步编程模型",
                    CreatedAt = DateTime.Now
                }
            };

            foreach (var question in questions)
            {
                context.Questions.Add(question);
            }

            await context.SaveChangesAsync();

            // 为选择题添加选项
            await CreateOptionsForQuestionsAsync(context);
        }

        /// <summary>
        /// 为选择题创建选项
        /// </summary>
        private static async Task CreateOptionsForQuestionsAsync(ExamSystemDbContext context)
        {
            var questions = await context.Questions
                .Where(q => q.QuestionType == QuestionType.SingleChoice || q.QuestionType == QuestionType.MultipleChoice)
                .ToListAsync();

            foreach (var question in questions)
            {
                if (question.Content.Contains("面向对象"))
                {
                    var options = new[]
                    {
                        new Option { QuestionId = question.QuestionId, Content = "抽象", IsCorrect = true, OrderIndex = 1 },
                        new Option { QuestionId = question.QuestionId, Content = "封装", IsCorrect = false, OrderIndex = 2 },
                        new Option { QuestionId = question.QuestionId, Content = "继承", IsCorrect = false, OrderIndex = 3 },
                        new Option { QuestionId = question.QuestionId, Content = "多态", IsCorrect = false, OrderIndex = 4 }
                    };
                    context.Options.AddRange(options);
                }
                else if (question.Content.Contains("string"))
                {
                    var options = new[]
                    {
                        new Option { QuestionId = question.QuestionId, Content = "值类型", IsCorrect = false, OrderIndex = 1 },
                        new Option { QuestionId = question.QuestionId, Content = "引用类型", IsCorrect = true, OrderIndex = 2 },
                        new Option { QuestionId = question.QuestionId, Content = "枚举类型", IsCorrect = false, OrderIndex = 3 },
                        new Option { QuestionId = question.QuestionId, Content = "结构类型", IsCorrect = false, OrderIndex = 4 }
                    };
                    context.Options.AddRange(options);
                }
                else if (question.Content.Contains("访问修饰符"))
                {
                    var options = new[]
                    {
                        new Option { QuestionId = question.QuestionId, Content = "public", IsCorrect = true, OrderIndex = 1 },
                        new Option { QuestionId = question.QuestionId, Content = "private", IsCorrect = true, OrderIndex = 2 },
                        new Option { QuestionId = question.QuestionId, Content = "protected", IsCorrect = true, OrderIndex = 3 },
                        new Option { QuestionId = question.QuestionId, Content = "internal", IsCorrect = true, OrderIndex = 4 }
                    };
                    context.Options.AddRange(options);
                }
                else if (question.Content.Contains("LINQ"))
                {
                    var options = new[]
                    {
                        new Option { QuestionId = question.QuestionId, Content = "数组", IsCorrect = true, OrderIndex = 1 },
                        new Option { QuestionId = question.QuestionId, Content = "集合", IsCorrect = true, OrderIndex = 2 },
                        new Option { QuestionId = question.QuestionId, Content = "数据库", IsCorrect = true, OrderIndex = 3 },
                        new Option { QuestionId = question.QuestionId, Content = "文本文件", IsCorrect = false, OrderIndex = 4 }
                    };
                    context.Options.AddRange(options);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
