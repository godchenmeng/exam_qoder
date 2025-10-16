using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ExamSystem.Repository.Context;
using ExamSystem.Infrastructure.Utils;

namespace ExamSystem.Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var dbPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "..", "..", "..", "..", // from Tools bin to solution root
                    "ExamSystem.UI", "bin", "Debug", "net5.0-windows", "exam_system.db");

                dbPath = Path.GetFullPath(dbPath);
                Console.WriteLine($"DB Path: {dbPath}");
                if (!File.Exists(dbPath))
                {
                    Console.WriteLine("Database file not found.");
                    return;
                }

                var options = new DbContextOptionsBuilder<ExamSystemDbContext>()
                    .UseSqlite($"Data Source={dbPath}")
                    .Options;

                using var db = new ExamSystemDbContext(options);
                // 运行初始化，确保修复旧默认哈希
                DbInitializer.InitializeAsync(db).GetAwaiter().GetResult();

                var admin = db.Users.FirstOrDefault(u => u.Username == "admin");
                if (admin == null)
                {
                    Console.WriteLine("Admin user not found.");
                    return;
                }

                Console.WriteLine($"Admin found | IsActive: {admin.IsActive} | Hash: {admin.PasswordHash}");

                var expected = PasswordHelper.HashPassword("admin123");
                Console.WriteLine($"Expected Hash: {expected}");
                var matches = string.Equals(admin.PasswordHash, expected);
                Console.WriteLine($"Match: {matches}");

                if (!matches && admin.IsActive)
                {
                    var legacy123456 = PasswordHelper.HashPassword("123456");
                    var legacyAdmin = PasswordHelper.HashPassword("admin");
                    if (admin.PasswordHash == legacy123456 || admin.PasswordHash == legacyAdmin)
                    {
                        admin.PasswordHash = expected;
                        db.Users.Update(admin);
                        db.SaveChanges();
                        Console.WriteLine("Admin password updated to admin123.");
                    }
                    else
                    {
                        Console.WriteLine("Admin password differs from expected and not recognized legacy; leaving unchanged.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}