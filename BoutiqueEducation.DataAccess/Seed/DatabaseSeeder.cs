using BoutiqueEducation.DataAccess.Context;
using BoutiqueEducation.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EntityTaskStatus = BoutiqueEducation.Entity.Entities.TaskStatus;

namespace BoutiqueEducation.DataAccess.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        AppDbContext context)
    {
        await SeedRolesAsync(roleManager);

        var admin   = await SeedAdminAsync(userManager);
        var teacher = await SeedTeacherAsync(userManager);
        var students = await SeedStudentsAsync(userManager);

        if (!await context.Questions.AnyAsync())
            await SeedQuestionsAsync(context, teacher, students);

        if (!await context.Tasks.AnyAsync())
            await SeedTasksAsync(context, teacher, students);
    }

    private static async Task SeedRolesAsync(RoleManager<AppRole> roleManager)
    {
        string[] roles = ["Admin", "Teacher", "Student", "Uye"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new AppRole { Name = role });
        }
    }

    private static async Task<AppUser> SeedAdminAsync(UserManager<AppUser> userManager)
    {
        const string email = "admin@boutique.edu";
        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null) return existing;

        var admin = new AppUser
        {
            UserName = email,
            Email = email,
            FullName = "Platform Yöneticisi",
            EmailConfirmed = true,
            Department = null,
            IsApproved = true
        };

        await userManager.CreateAsync(admin, "Admin@123456");
        await userManager.AddToRoleAsync(admin, "Admin");
        return admin;
    }

    private static async Task<AppUser> SeedTeacherAsync(UserManager<AppUser> userManager)
    {
        const string email = "ogretmen@boutique.edu";
        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null) return existing;

        var teacher = new AppUser
        {
            UserName = email,
            Email = email,
            FullName = "Ayşe Kaya",
            EmailConfirmed = true,
            Department = "Matematik",
            IsApproved = true
        };

        await userManager.CreateAsync(teacher, "Teacher@123");
        await userManager.AddToRoleAsync(teacher, "Teacher");
        return teacher;
    }

    private static async Task<List<AppUser>> SeedStudentsAsync(UserManager<AppUser> userManager)
    {
        var studentData = new[]
        {
            ("ogrenci1@boutique.edu", "Ali Veli"),
            ("ogrenci2@boutique.edu", "Zeynep Çelik"),
            ("ogrenci3@boutique.edu", "Mehmet Yıldız"),
        };

        var students = new List<AppUser>();
        foreach (var (email, fullName) in studentData)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing != null) { students.Add(existing); continue; }

            var student = new AppUser
            {
                UserName = email, Email = email,
                FullName = fullName, EmailConfirmed = true,
                IsApproved = true
            };

            await userManager.CreateAsync(student, "Student@123");
            await userManager.AddToRoleAsync(student, "Student");
            students.Add(student);
        }

        return students;
    }

    private static async Task SeedQuestionsAsync(AppDbContext context, AppUser teacher, List<AppUser> students)
    {
        var questions = new List<Question>
        {
            new()
            {
                StudentId = students[0].Id,
                TeacherId = teacher.Id,
                Content = "Türevin geometrik anlamı nedir?",
                Category = "Matematik",
                Status = QuestionStatus.Answered,
                AnswerText = "Fonksiyonun o noktadaki anlık değişim oranını (eğimini) gösterir.",
                AnsweredAt = DateTime.UtcNow.AddDays(-2),
                CreatedDate = DateTime.UtcNow.AddDays(-3)
            },
            new()
            {
                StudentId = students[1].Id,
                Content = "Fotosentez hangi organelde gerçekleşir?",
                Category = "Biyoloji",
                Status = QuestionStatus.Pending,
                CreatedDate = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                StudentId = students[2].Id,
                Content = "Newton'un hareket yasaları nelerdir?",
                Category = "Fizik",
                Status = QuestionStatus.Pending,
                CreatedDate = DateTime.UtcNow.AddHours(-5)
            },
            new()
            {
                StudentId = students[0].Id,
                TeacherId = teacher.Id,
                Content = "İntegral ile türev arasındaki ilişki nedir?",
                Category = "Matematik",
                Status = QuestionStatus.Answered,
                AnswerText = "Temel Calculus Teoremi: türev ve integral birbirinin tersi işlemlerdir.",
                AnsweredAt = DateTime.UtcNow.AddDays(-1),
                CreatedDate = DateTime.UtcNow.AddDays(-2)
            }
        };

        await context.Questions.AddRangeAsync(questions);
        await context.SaveChangesAsync();
    }

    private static async Task SeedTasksAsync(AppDbContext context, AppUser teacher, List<AppUser> students)
    {
        var tasks = new List<TaskItem>
        {
            new()
            {
                Title = "Matematik Deneme Sınavı 1",
                Description = "Sayfa 45-67 arasındaki soruları çöz. Türev ve integral konularına odaklan.",
                TeacherId = teacher.Id,
                StudentId = students[0].Id,
                Status = EntityTaskStatus.Approved,
                CreatedDate = DateTime.UtcNow.AddDays(-10),
                SubmittedAt = DateTime.UtcNow.AddDays(-6),
                ApprovedAt = DateTime.UtcNow.AddDays(-5)
            },
            new()
            {
                Title = "Fizik Lab Raporu",
                Description = "Geçen haftaki deney sonuçlarını raporla.",
                TeacherId = teacher.Id,
                StudentId = students[1].Id,
                Status = EntityTaskStatus.Submitted,
                CreatedDate = DateTime.UtcNow.AddDays(-7),
                SubmittedAt = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                Title = "Biyoloji Araştırma Ödevi",
                Description = "Fotosentez mekanizmasını detaylı araştır.",
                TeacherId = teacher.Id,
                StudentId = students[2].Id,
                Status = EntityTaskStatus.Pending,
                CreatedDate = DateTime.UtcNow.AddDays(-3)
            }
        };

        await context.Tasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
    }
}
