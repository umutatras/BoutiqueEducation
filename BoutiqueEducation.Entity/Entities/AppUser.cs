using Microsoft.AspNetCore.Identity;


namespace BoutiqueEducation.Entity.Entities;

using BoutiqueEducation.Entity.Interfaces;

public sealed class AppUser : IdentityUser<Guid>, ICreatableEntity, IUpdatableEntity, ISoftDeletableEntity
{
    public string FullName { get; set; } = null!;
    public string? GoogleProviderId { get; set; }
    /// <summary>Öğretmenler için bölüm (Matematik, Fen, Biyoloji, vb.)</summary>
    public string? Department { get; set; }
    /// <summary>Admin tarafından onaylanmış mı?</summary>
    public bool IsApproved { get; set; } = false;

    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public Guid? CreatedById { get; set; }

    public DateTime? UpdatedDate { get; set; }
    public Guid? ModifiedById { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public Guid? DeletedById { get; set; }

    // Navigation properties for relationships

    public ICollection<Question> QuestionsAsStudent { get; set; } = new List<Question>();


    public ICollection<Question> AnswersAsTeacher { get; set; } = new List<Question>();


    public ICollection<TaskItem> TasksGivenAsTeacher { get; set; } = new List<TaskItem>();


    public ICollection<TaskItem> TasksReceivedAsStudent { get; set; } = new List<TaskItem>();


    public ICollection<Message> SentMessages { get; set; } = new List<Message>();


    public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
}
