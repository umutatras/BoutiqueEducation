using BoutiqueEducation.Entity.Interfaces;

namespace BoutiqueEducation.Entity.Entities;

public sealed class Question : BaseEntity, ICreatableEntity, IUpdatableEntity, ISoftDeletableEntity
{
    public string Content { get; set; } = string.Empty;  // Soru metni
    public string? ImageUrl { get; set; }  // Opsiyonel görsel
    public string Category { get; set; } = "Genel"; // Matematik, Fen, Biyoloji, vb.

    // Status can be Enum
    public QuestionStatus Status { get; set; } = QuestionStatus.Pending;

    public Guid StudentId { get; set; }
    public AppUser Student { get; set; } = null!;

    public Guid? TeacherId { get; set; }
    public AppUser? Teacher { get; set; }

    public string? AnswerText { get; set; }
    public string? AnswerImageUrl { get; set; }
    public DateTime? AnsweredAt { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public Guid? CreatedById { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public Guid? ModifiedById { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public Guid? DeletedById { get; set; }
}

public enum QuestionStatus
{
    Pending,
    Answered,
    Rejected
}
