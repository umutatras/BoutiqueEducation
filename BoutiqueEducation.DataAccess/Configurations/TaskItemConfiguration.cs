using BoutiqueEducation.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BoutiqueEducation.DataAccess.Configurations;

public sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.HasQueryFilter(t => !t.IsDeleted);

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(t => t.Student)
            .WithMany(u => u.TasksReceivedAsStudent)
            .HasForeignKey(t => t.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Teacher)
            .WithMany(u => u.TasksGivenAsTeacher)
            .HasForeignKey(t => t.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
