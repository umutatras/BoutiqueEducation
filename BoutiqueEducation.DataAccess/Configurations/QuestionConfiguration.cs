using BoutiqueEducation.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BoutiqueEducation.DataAccess.Configurations;

public sealed class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasQueryFilter(q => !q.IsDeleted);

        builder.HasKey(q => q.Id);

        builder.Property(q => q.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(q => q.Student)
            .WithMany(s => s.QuestionsAsStudent)
            .HasForeignKey(q => q.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(q => q.Teacher)
            .WithMany(t => t.AnswersAsTeacher)
            .HasForeignKey(q => q.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
