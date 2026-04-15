using BoutiqueEducation.Entity.Interfaces;

namespace BoutiqueEducation.Entity.Entities;

public sealed class Message : BaseEntity, ICreatableEntity, IUpdatableEntity, ISoftDeletableEntity
{
    public string Content { get; set; } = null!;
    public Guid SenderId { get; set; }
    public AppUser Sender { get; set; } = null!;

    public Guid ReceiverId { get; set; }
    public AppUser Receiver { get; set; } = null!;

    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public Guid? CreatedById { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public Guid? ModifiedById { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public Guid? DeletedById { get; set; }
}
