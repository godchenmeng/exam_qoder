using Microsoft.EntityFrameworkCore;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Enums;

namespace ExamSystem.Repository.Context
{
    /// <summary>
    /// 考试系统数据库上下文
    /// </summary>
    public class ExamSystemDbContext : DbContext
    {
        public ExamSystemDbContext(DbContextOptions<ExamSystemDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<QuestionBank> QuestionBanks { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<ExamPaper> ExamPapers { get; set; }
        public DbSet<PaperQuestion> PaperQuestions { get; set; }
        public DbSet<ExamRecord> ExamRecords { get; set; }
        public DbSet<AnswerRecord> AnswerRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User 配置
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(256);
                entity.Property(e => e.RealName).HasMaxLength(50);
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            // QuestionBank 配置
            modelBuilder.Entity<QuestionBank>(entity =>
            {
                entity.HasKey(e => e.BankId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.CreatorId).IsRequired();
                entity.Property(e => e.IsPublic).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasOne(e => e.Creator)
                    .WithMany()
                    .HasForeignKey(e => e.CreatorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Question 配置
            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(e => e.QuestionId);
                entity.Property(e => e.BankId).IsRequired();
                entity.Property(e => e.QuestionType).IsRequired();
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.Answer).IsRequired();
                entity.Property(e => e.DefaultScore).IsRequired().HasColumnType("decimal(5,2)");
                entity.Property(e => e.Difficulty).IsRequired();
                entity.Property(e => e.Tags).HasMaxLength(200);
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasOne(e => e.QuestionBank)
                    .WithMany()
                    .HasForeignKey(e => e.BankId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Option 配置
            modelBuilder.Entity<Option>(entity =>
            {
                entity.HasKey(e => e.OptionId);
                entity.Property(e => e.QuestionId).IsRequired();
                entity.Property(e => e.Content).IsRequired().HasMaxLength(500);
                entity.Property(e => e.IsCorrect).IsRequired();
                entity.Property(e => e.OrderIndex).IsRequired();

                entity.HasOne(e => e.Question)
                    .WithMany()
                    .HasForeignKey(e => e.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ExamPaper 配置
            modelBuilder.Entity<ExamPaper>(entity =>
            {
                entity.HasKey(e => e.PaperId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.TotalScore).IsRequired().HasColumnType("decimal(6,2)");
                entity.Property(e => e.PassScore).IsRequired().HasColumnType("decimal(6,2)");
                entity.Property(e => e.Duration).IsRequired();
                entity.Property(e => e.PaperType).IsRequired();
                entity.Property(e => e.CreatorId).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasOne(e => e.Creator)
                    .WithMany()
                    .HasForeignKey(e => e.CreatorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PaperQuestion 配置
            modelBuilder.Entity<PaperQuestion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PaperId).IsRequired();
                entity.Property(e => e.QuestionId).IsRequired();
                entity.Property(e => e.OrderIndex).IsRequired();
                entity.Property(e => e.Score).IsRequired().HasColumnType("decimal(5,2)");

                entity.HasOne(e => e.ExamPaper)
                    .WithMany()
                    .HasForeignKey(e => e.PaperId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Question)
                    .WithMany()
                    .HasForeignKey(e => e.QuestionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ExamRecord 配置
            modelBuilder.Entity<ExamRecord>(entity =>
            {
                entity.HasKey(e => e.RecordId);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.PaperId).IsRequired();
                entity.Property(e => e.StartTime).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.TotalScore).HasColumnType("decimal(6,2)");
                entity.Property(e => e.ObjectiveScore).HasColumnType("decimal(6,2)");
                entity.Property(e => e.SubjectiveScore).HasColumnType("decimal(6,2)");
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ExamPaper)
                    .WithMany()
                    .HasForeignKey(e => e.PaperId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // AnswerRecord 配置
            modelBuilder.Entity<AnswerRecord>(entity =>
            {
                entity.HasKey(e => e.AnswerId);
                entity.Property(e => e.RecordId).IsRequired();
                entity.Property(e => e.QuestionId).IsRequired();
                entity.Property(e => e.Score).HasColumnType("decimal(5,2)");
                entity.Property(e => e.IsGraded).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.GradeComment).HasMaxLength(500);

                entity.HasOne(e => e.ExamRecord)
                    .WithMany()
                    .HasForeignKey(e => e.RecordId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Question)
                    .WithMany()
                    .HasForeignKey(e => e.QuestionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Grader)
                    .WithMany()
                    .HasForeignKey(e => e.GraderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // 种子数据 - 默认管理员账户
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Username = "admin",
                    PasswordHash = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=", // admin123的SHA256哈希
                    RealName = "系统管理员",
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = System.DateTime.Now
                }
            );
        }
    }
}
