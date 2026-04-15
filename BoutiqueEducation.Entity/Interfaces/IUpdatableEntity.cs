namespace BoutiqueEducation.Entity.Interfaces;

public interface IUpdatableEntity
{
    DateTime? UpdatedDate { get; set; }
    Guid? ModifiedById { get; set; }
}
