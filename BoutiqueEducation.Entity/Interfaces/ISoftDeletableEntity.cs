namespace BoutiqueEducation.Entity.Interfaces;

public interface ISoftDeletableEntity
{
    bool IsDeleted { get; set; }
    DateTime? DeletedDate { get; set; }
    Guid? DeletedById { get; set; }
}
