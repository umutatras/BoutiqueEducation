using BoutiqueEducation.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BoutiqueEducation.DataAccess.Configurations;

public sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasQueryFilter(u => !u.IsDeleted);

        // Navigation properties
        builder.HasMany(u => u.QuestionsAsStudent)
               .WithOne(q => q.Student)
               .HasForeignKey(q => q.StudentId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.AnswersAsTeacher)
               .WithOne(q => q.Teacher)
               .HasForeignKey(q => q.TeacherId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.TasksGivenAsTeacher)
               .WithOne(t => t.Teacher)
               .HasForeignKey(t => t.TeacherId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.TasksReceivedAsStudent)
               .WithOne(t => t.Student)
               .HasForeignKey(t => t.StudentId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.SentMessages)
               .WithOne(m => m.Sender)
               .HasForeignKey(m => m.SenderId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.ReceivedMessages)
               .WithOne(m => m.Receiver)
               .HasForeignKey(m => m.ReceiverId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
