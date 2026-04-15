using BoutiqueEducation.Entity.Interfaces;

namespace BoutiqueEducation.Entity.Entities;

public sealed class TaskItem : BaseEntity, ICreatableEntity, IUpdatableEntity, ISoftDeletableEntity
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public TaskStatus Status { get; set; } = TaskStatus.Pending;

    public DateTime? SubmittedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? DueDate { get; set; } // Takvimde gösterilecek son teslim tarihi

    public Guid TeacherId { get; set; }
    public AppUser Teacher { get; set; } = null!;

    public Guid StudentId { get; set; }
    public AppUser Student { get; set; } = null!;

    public string? SubmissionImageUrl { get; set; }
    public string? SubmissionFileUrl { get; set; } // PDF dosya yolu
    public string? SubmissionNotes { get; set; } // Öğrencinin teslim notu

    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public Guid? CreatedById { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public Guid? ModifiedById { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public Guid? DeletedById { get; set; }
}

public enum TaskStatus
{
    Pending,
    Submitted,
    Approved,
    Rejected
}
