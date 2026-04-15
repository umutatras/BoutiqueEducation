namespace BoutiqueEducation.Entity.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
}
